DO $$
DECLARE
    -- Account Types
    ACCT_TYPE_ASSET INTEGER := (SELECT "Id" FROM "AccountType" WHERE "Name" = 'Asset');
    ACCT_TYPE_LIABILITY INTEGER := (SELECT "Id" FROM "AccountType" WHERE "Name" = 'Liability');
    ACCT_TYPE_EQUITY INTEGER := (SELECT "Id" FROM "AccountType" WHERE "Name" = 'Equity');
    ACCT_TYPE_REVENUE INTEGER := (SELECT "Id" FROM "AccountType" WHERE "Name" = 'Revenue');
    ACCT_TYPE_EXPENSE INTEGER := (SELECT "Id" FROM "AccountType" WHERE "Name" = 'Expense');
    ACCT_TYPE_CONTRA INTEGER := (SELECT "Id" FROM "AccountType" WHERE "Name" = 'Contra');

    -- Asset Type for U.S. Dollars
    ASSET_TYPE_USD INTEGER := (SELECT "Id" FROM "AssetType" WHERE "Name" = 'USD $');

    -- Balance Types
    BALANCE_TYPE_DEBIT SMALLINT := 1;
    BALANCE_TYPE_CREDIT SMALLINT := -1;

    -- Created By User
    GEOFFREY TEXT := (SELECT "Id" FROM "AspNetUsers" WHERE "NormalizedUserName" = 'GROBERTS314@YAHOO.COM');

    the_tenant_id INTEGER;
BEGIN
    SELECT "Id" FROM "Tenant" WHERE LOWER("Name") = LOWER('Example Corporation') LIMIT 1 INTO the_tenant_id;

    IF the_tenant_id IS NULL THEN
        INSERT INTO "Tenant" ( "Name" ) VALUES ( 'Example Corporation' )
        RETURNING "Id" INTO STRICT the_tenant_id;
    END IF;

    -- Account 1010 - Cash Operating - Type: Asset, Normal Balance: Debit
    IF NOT EXISTS (
        SELECT 1 FROM "Account" WHERE "TenantId" = the_tenant_id AND LOWER("Name") = LOWER('Cash Operating') LIMIT 1
    ) THEN
        INSERT INTO "Account"
        (
             "TenantId"
            ,"AccountNumber"
            ,"Name"
            ,"Description"
            ,"AccountTypeId"
            ,"AssetTypeId"
            ,"NormalBalanceType"
            ,"CurrentBalance"
            ,"BalanceUpdated"
            ,"CreatedById"
        )
        VALUES
        (
             the_tenant_id
            ,1010
            ,'Cash Operating'
            ,'Primary Cash Operating Account.'
            ,ACCT_TYPE_ASSET
            ,ASSET_TYPE_USD
            ,BALANCE_TYPE_DEBIT
            ,0
            ,now() AT TIME ZONE 'UTC'
            ,GEOFFREY
        );
    END IF;
END
$$ LANGUAGE plpgsql;