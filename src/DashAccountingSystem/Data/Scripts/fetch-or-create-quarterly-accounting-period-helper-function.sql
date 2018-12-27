-- ===========================================================================
-- Helper function to create or retreive existing quarterly accounting
-- period.
-- ===========================================================================
DROP FUNCTION IF EXISTS fetch_or_create_quarterly_accounting_period (INTEGER, TIMESTAMP);
DROP FUNCTION IF EXISTS fetch_or_create_quarterly_accounting_period (INTEGER, TIMESTAMP, OUT INTEGER);

CREATE OR REPLACE FUNCTION fetch_or_create_quarterly_accounting_period (
    the_tenant_id INTEGER
   ,the_date TIMESTAMP
)
RETURNS INTEGER
AS $$
DECLARE
    QUARTERLY_PERIOD_TYPE SMALLINT := 1;
    the_year INTEGER;
    the_quarter SMALLINT;
    the_period_id INTEGER;
BEGIN
    the_year := (SELECT EXTRACT(YEAR FROM the_date));
    the_quarter := (SELECT EXTRACT(QUARTER FROM the_date));
    
    SELECT "Id" FROM "AccountingPeriod"
     WHERE "PeriodType" = QUARTERLY_PERIOD_TYPE
       AND "Year" = the_year
       AND "Quarter" = the_quarter
      INTO the_period_id;
  
    IF the_period_id IS NULL THEN
        INSERT INTO "AccountingPeriod" ( "Name", "TenantId", "PeriodType", "Year", "Month", "Quarter" )
        VALUES
        (
             the_year::VARCHAR || ' Q' || the_quarter::VARCHAR
            ,the_tenant_id
            ,QUARTERLY_PERIOD_TYPE
            ,the_year
            ,the_quarter * 3
            ,the_quarter
        )
        RETURNING "Id"
        INTO STRICT the_period_id;
    END IF;

    RETURN the_period_id;
END
$$ LANGUAGE plpgsql;