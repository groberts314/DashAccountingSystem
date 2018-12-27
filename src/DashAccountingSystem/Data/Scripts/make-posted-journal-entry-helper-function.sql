-- ===========================================================================
-- Helper function to create journal entries/transaction
-- ===========================================================================
DO $$
BEGIN
    IF NOT EXISTS ( SELECT 1 FROM pg_type WHERE typname = 'journal_entry_account' ) THEN
        CREATE TYPE journal_entry_account AS
        (
            account_id INTEGER,
            asset_type_id INTEGER,
            amount NUMERIC
        );
    END IF;
END
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION make_posted_journal_entry (
    tenant_id INTEGER
   ,entry_date TIMESTAMP
   ,post_date TIMESTAMP
   ,description VARCHAR(2048)
   ,check_number SMALLINT
   ,user_id UUID
   ,note TEXT
   ,accounts journal_entry_account[]
)
RETURNS INTEGER
AS $$
DECLARE
    accounting_period_id INTEGER;
    accounting_period_name VARCHAR;
    entry_number INTEGER;
    journal_entry_id INTEGER;
    account_number INTEGER;
    account_name VARCHAR;
    account_current_balance NUMERIC;
    current_account journal_entry_account;
    transaction_timestamp TIMESTAMPTZ;
    new_balance NUMERIC;
    STATUS_POSTED INTEGER := 2;
BEGIN
    -- Resolve the Accounting Period
    SELECT fetch_or_create_quarterly_accounting_period(tenant_id, post_date)
    INTO STRICT accounting_period_id;

    SELECT "Name" FROM "AccountingPeriod"
    WHERE "Id" = accounting_period_id
    INTO STRICT accounting_period_name;

    RAISE NOTICE 'Accounting Period ID % - %', accounting_period_id, accounting_period_name;
    
    -- Resolve next available Journal Entry/Transaction Number
    SELECT COALESCE(MAX("EntryId"),0) + 1 FROM "JournalEntry"
    WHERE "TenantId" = tenant_id
    INTO STRICT entry_number;

    RAISE NOTICE 'Journal Entry / Transaction # %', entry_number;

    transaction_timestamp := (now() AT TIME ZONE 'UTC');
    
    -- Insert Journal Entry
    INSERT INTO "JournalEntry"
    (
         "TenantId"
        ,"EntryId"
        ,"EntryDate"
        ,"PostDate"
        ,"AccountingPeriodId"
        ,"Description"
        ,"CheckNumber"
        ,"Note"
        ,"Status"
        ,"Created"
        ,"CreatedById"
        ,"PostedById"
    )
    VALUES
    (
         tenant_id
        ,entry_number
        ,entry_date
        ,post_date
        ,accounting_period_id
        ,description
        ,check_number
        ,note
        ,STATUS_POSTED
        ,transaction_timestamp
        ,user_id
        ,user_id
    )
    RETURNING "Id" INTO STRICT journal_entry_id;
    
    -- For Each Account
    FOREACH current_account IN ARRAY accounts LOOP
        -- Resolve Account Name, Account Number and Current Balance
        SELECT "AccountNumber", "Name", "CurrentBalance"
        FROM "Account"
        WHERE "Id" = current_account.account_id
        INTO STRICT account_number, account_name, account_current_balance;
    
        new_balance := account_current_balance + current_account.amount;
    
        RAISE NOTICE 'Account ID % / % - % / Previous Balance: % / New Balance: %'
            ,current_account.account_id
            ,account_number
            ,account_name
            ,CAST(account_current_balance AS money)
            ,CAST(new_balance AS money);
        
        -- Insert into JournalEntryAccount table
        INSERT INTO "JournalEntryAccount"
        ( "JournalEntryId", "AccountId", "AssetTypeId", "Amount", "PreviousBalance", "NewBalance" )
        VALUES
        (
             journal_entry_id
            ,current_account.account_id
            ,current_account.asset_type_id
            ,current_account.amount
            ,account_current_balance
            ,new_balance
        );
        
        -- Update Account Balance
        UPDATE "Account"
           SET "CurrentBalance" = new_balance
              ,"BalanceUpdated" = transaction_timestamp
         WHERE "Id" = current_account.account_id;
        
    END LOOP;

    RETURN journal_entry_id;
END
$$ LANGUAGE plpgsql;