DO $$
BEGIN
    -- JournalEntryAccount
    IF ( SELECT COALESCE(MAX("Id"), 0) FROM "JournalEntryAccount" ) = 0 THEN
        ALTER SEQUENCE "JournalEntryAccount_Id_seq" RESTART WITH 1;
    ELSE
        PERFORM setval(
            pg_get_serial_sequence('public."JournalEntryAccount"', 'Id'),
            (SELECT MAX("Id") FROM "JournalEntryAccount"));    
    END IF;

    -- JournalEntry
    IF ( SELECT COALESCE(MAX("Id"), 0) FROM "JournalEntry" ) = 0 THEN
        ALTER SEQUENCE "JournalEntry_Id_seq" RESTART WITH 1;
    ELSE
        PERFORM setval(
            pg_get_serial_sequence('public."JournalEntry"', 'Id'),
            (SELECT MAX("Id") FROM "JournalEntry"));    
    END IF;

    -- Account
    IF ( SELECT COALESCE(MAX("Id"), 0) FROM "Account" ) = 0 THEN
        ALTER SEQUENCE "Account_Id_seq" RESTART WITH 1;
    ELSE
        PERFORM setval(
            pg_get_serial_sequence('public."Account"', 'Id'),
            (SELECT MAX("Id") FROM "Account"));    
    END IF;

    -- AccountingPeriod
    IF ( SELECT COALESCE(MAX("Id"), 0) FROM "AccountingPeriod" ) = 0 THEN
        ALTER SEQUENCE "AccountingPeriod_Id_seq" RESTART WITH 1;
    ELSE
        PERFORM setval(
            pg_get_serial_sequence('public."AccountingPeriod"', 'Id'),
            (SELECT MAX("Id") FROM "AccountingPeriod"));    
    END IF;

    -- Tenant
    IF ( SELECT COALESCE(MAX("Id"), 0) FROM "Tenant" ) = 0 THEN
        ALTER SEQUENCE "Tenant_Id_seq" RESTART WITH 1;
    ELSE
        PERFORM setval(
            pg_get_serial_sequence('public."Tenant"', 'Id'),
            (SELECT MAX("Id") FROM "Tenant"));    
    END IF;
END $$ LANGUAGE plpgsql;