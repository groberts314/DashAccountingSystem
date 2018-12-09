DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM "Tenant" WHERE "Name" = 'Example Corporation' LIMIT 1) THEN
        INSERT INTO "Tenant" ( "Name" ) VALUES ( 'Example Corporation' );
    END IF;

    IF NOT EXISTS (SELECT 1 FROM "Tenant" WHERE "Name" = 'Dash Software Solutions, Inc.' LIMIT 1) THEN
        INSERT INTO "Tenant" ( "Name" ) VALUES ( 'Dash Software Solutions, Inc.' );
    END IF;
END
$$ LANGUAGE plpgsql;