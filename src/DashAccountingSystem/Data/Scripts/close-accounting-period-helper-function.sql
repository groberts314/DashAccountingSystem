-- ===========================================================================
-- Helper function to close out an accounting period.
-- * Adds Period Closing Blances entries for all Accounts except Retained
--   Earnings.
-- * Posts a Closing Journal Entry which zeroes out all Revenue and Expense
--   accounts and balances them with debits and credits to retained earnings
--   account.
-- * Adds Period Closing Blances entry for Retained Earnings
-- * Sets Closed Status on all Journal Entries for the Period
-- REMARKS:
-- * This is currently very hard-coded for the simple case of a single
--   owner, and cheats a bit to identify the Retained Earnings Account.
-- * In the future it may be necessary to come up with a better way to
--   identify Retained Earnings accounts and figure out how to allocate
--   Revenue and Expenses among multiple owners' retained earnings.
-- ===========================================================================
CREATE OR REPLACE FUNCTION close_accounting_period(the_accounting_period_id INTEGER)
RETURNS INTEGER
AS $$
DECLARE
    ASSET_TYPE_USD INTEGER := (SELECT "Id" FROM "AssetType" WHERE "Name" = 'USD $');
    GEOFFREY UUID := (SELECT "Id" FROM "AspNetUsers" WHERE "NormalizedUserName" = 'GROBERTS314@YAHOO.COM');
    STATUS_CLOSED SMALLINT := 4;
    the_tenant_id INTEGER;
    the_tenant_name VARCHAR;
    the_period_name VARCHAR;
    the_period_year INTEGER;
    the_period_month SMALLINT;
    current_account RECORD;
    account_cursor refcursor;
    re_acct_id INTEGER;
    re_acct_name VARCHAR;
    re_acct_number INTEGER;
    retained_earnings_amount NUMERIC := 0;
    retained_earnings_new_balance NUMERIC;
    closing_entry_accounts journal_entry_account[] := ARRAY[]::journal_entry_account[];
    period_close_date TIMESTAMP;
BEGIN
    -- Resolve Accounting Period and Tenant
    SELECT ap."TenantId"
          ,ap."Name" AS accounting_period_name
          ,ap."Year" AS accounting_period_year
          ,ap."Month" AS accounting_period_month
          ,t."Name" AS tenant_name
      FROM "AccountingPeriod" ap JOIN "Tenant" t ON ap."TenantId" = t."Id"
     WHERE ap."Id" = the_accounting_period_id
    INTO STRICT the_tenant_id, the_period_name, the_period_year, the_period_month, the_tenant_name;

    RAISE NOTICE 'Found Accounting Period ID % - % - for Tenant ID % - %'
        ,the_accounting_period_id
        ,the_period_name
        ,the_tenant_id
        ,the_tenant_name;
    
    -- Resolve Retained Earnings Account
    SELECT "Id","Name","AccountNumber" FROM "Account"
     WHERE "TenantId" = the_tenant_id
       AND "Name" ILIKE '%Retained Earnings%'
    INTO STRICT re_acct_id, re_acct_name, re_acct_number;

    RAISE NOTICE 'Found Retained Earnings Account: ID % - % - %'
        ,re_acct_id
        ,re_acct_name
        ,re_acct_number;
    
    -- Setup a cursor over all the accounts for this tenant
    OPEN account_cursor FOR
    SELECT a."Id" AS account_id
          ,a."AccountNumber" AS account_number
          ,a."Name" AS account_name
          ,at."Name" AS account_type
          ,a."CurrentBalance" AS current_balance
      FROM "Account" a JOIN "AccountType" at ON a."AccountTypeId" = at."Id"
     WHERE a."TenantId" = the_tenant_id
       AND a."Id" <> re_acct_id
    ORDER BY a."AccountNumber";

    -- Loop over the accounts
    LOOP
        FETCH account_cursor INTO current_account;
        EXIT WHEN NOT FOUND;
    
        RAISE NOTICE 'Processing Account ID % / % - % / Current Balance: % ...'
            ,current_account.account_id
            ,current_account.account_number
            ,current_account.account_name
            ,CAST(current_account.current_balance AS money);
        
        -- Insert Period Closing Balance Record
        INSERT INTO "AccountingPeriodClosingBalance" ( "AccountId", "AccountingPeriodId", "ClosingBalance" )
        VALUES ( current_account.account_id, the_accounting_period_id, current_account.current_balance );
    
        RAISE NOTICE '> Account Type is %', current_account.account_type;
    
        -- If it's a Revenue or Expense Account
        IF ( current_account.account_type = 'Revenue'
             OR current_account.account_type = 'Expense' )
            AND current_account.current_balance <> 0
        THEN
            -- Add Journal Entry Account that will zero it
            closing_entry_accounts := array_append(
                 closing_entry_accounts
                ,ROW(current_account.account_id, ASSET_TYPE_USD, 0 - current_account.current_balance)::journal_entry_account);
            
            -- Add its amount to retained earnings amount
            retained_earnings_amount := retained_earnings_amount + current_account.current_balance;
        END IF;
    END LOOP;
        
    CLOSE account_cursor;

    RAISE NOTICE 'Retained Earnings Amount is: %', CAST(retained_earnings_amount AS money);

    -- Add Retained Earnings to Closing Journal Entry Accounts
    closing_entry_accounts := array_append(
         closing_entry_accounts
        ,ROW(re_acct_id, ASSET_TYPE_USD, retained_earnings_amount)::journal_entry_account);

    -- Get date for last day in the accounting period
    period_close_date := make_date(the_period_year, the_period_month, 1)::TIMESTAMP + INTERVAL '1 Month' - INTERVAL '1 Day';
    RAISE NOTICE 'Closing Journal Entry will be entered/posted on %', period_close_date;

    -- Make Closing Journal Entry
    PERFORM make_posted_journal_entry(
         the_tenant_id
        ,period_close_date
        ,period_close_date
        ,'Closing entry for ' || the_period_name
        ,NULL
        ,GEOFFREY
        ,NULL
        ,closing_entry_accounts);
    
    -- Fetch upated balance from Retained Earnings Account
    SELECT "CurrentBalance" FROM "Account" WHERE "Id" = re_acct_id
    INTO STRICT retained_earnings_new_balance;
    
    -- Add Retained Earnings Closing Balance
    INSERT INTO "AccountingPeriodClosingBalance" ( "AccountId", "AccountingPeriodId", "ClosingBalance" )
    VALUES ( re_acct_id, the_accounting_period_id, retained_earnings_new_balance );
    
    -- Set Status of all Journal Entries for the Period to Closed
    UPDATE "JournalEntry"
       SET "Status" = STATUS_CLOSED
          ,"Updated" = (now() AT TIME ZONE 'UTC')
          ,"UpdatedById" = GEOFFREY
     WHERE "AccountingPeriodId" = the_accounting_period_id;
 
    -- Mark the Period as Closed
    UPDATE "AccountingPeriod"
       SET "Closed" = TRUE
     WHERE "Id" = the_accounting_period_id;

    RETURN 0;
END
$$ LANGUAGE plpgsql;