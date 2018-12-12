using System;
using Microsoft.EntityFrameworkCore;
using Dapper;
using Npgsql;
using Xunit;
using DashAccountingSystem.Data.Repositories;

namespace DashAccountingSystem.Tests
{
    public class AccountingPeriodRepositoryFixture
    {
        private int _tenantId = 0;

        [Fact]
        [Trait("Category", "Requires Database")]
        public void FetchOrCreateAccountPeriod_Ok()
        {
            lock (TestUtilities.DatabaseSyncLock)
            {
                try
                {
                    InitializeTenant();
                    Assert.True(_tenantId > 0);

                    var repository = GetAccountingPeriodRepository();
                    var year = 2018;
                    var month = (byte)12;
                    var accountingPeriod0 = repository.FetchOrCreateAccountPeriodAsync(_tenantId, year, month).Result;
                    Assert.NotNull(accountingPeriod0);
                    Assert.True(accountingPeriod0.Id > 0);
                    Assert.Equal(_tenantId, accountingPeriod0.TenantId);
                    Assert.Equal(year, accountingPeriod0.Year);
                    Assert.Equal(month, accountingPeriod0.Month);

                    var fetchedAccountingPeriod0 = repository.FetchOrCreateAccountPeriodAsync(_tenantId, year, month).Result;
                    Assert.Equal(accountingPeriod0.Id, fetchedAccountingPeriod0.Id);
                    Assert.Equal(_tenantId, fetchedAccountingPeriod0.TenantId);
                    Assert.Equal(year, fetchedAccountingPeriod0.Year);
                    Assert.Equal(month, fetchedAccountingPeriod0.Month);

                    year = 2019;
                    month = 1;
                    var date = new DateTime(year, month, 27);
                    var accountingPeriod1 = repository.FetchOrCreateAccountPeriodAsync(_tenantId, date).Result;
                    Assert.NotNull(accountingPeriod1);
                    Assert.True(accountingPeriod1.Id > 0);
                    Assert.Equal(_tenantId, accountingPeriod1.TenantId);
                    Assert.Equal(year, accountingPeriod1.Year);
                    Assert.Equal(month, accountingPeriod1.Month);

                    var fetchedAccountingPeriod1 = repository.FetchOrCreateAccountPeriodAsync(_tenantId, date).Result;
                    Assert.Equal(accountingPeriod1.Id, fetchedAccountingPeriod1.Id);
                    Assert.Equal(_tenantId, fetchedAccountingPeriod1.TenantId);
                    Assert.Equal(year, fetchedAccountingPeriod1.Year);
                    Assert.Equal(month, fetchedAccountingPeriod1.Month);
                }
                finally
                {
                    Cleanup();
                }
            }
        }

        private void InitializeTenant()
        {
            var connString = TestUtilities.GetConnectionString();

            using (var connection = new NpgsqlConnection(connString))
            {
                _tenantId = connection.QueryFirstOrDefault<int>(@"
                    INSERT INTO ""Tenant"" ( ""Name"" ) VALUES ( 'Unit Testing, Inc.' ) RETURNING ""Id"";");
            }
        }

        private void Cleanup()
        {
            var connString = TestUtilities.GetConnectionString();

            using (var connection = new NpgsqlConnection(connString))
            {
                var parameters = new { _tenantId };

                connection.Execute(@"
                    DELETE FROM ""AccountingPeriod"" WHERE ""TenantId"" = @_tenantId;",
                    parameters);

                connection.Execute(@"
                    DELETE FROM ""Tenant"" WHERE ""Id"" = @_tenantId;",
                    parameters);

                connection.Execute(@"
                    DO $$
                    BEGIN
                        -- AccountingPeriod
                        IF ( SELECT COALESCE(MAX(""Id""), 0) FROM ""AccountingPeriod"" ) = 0 THEN
                            ALTER SEQUENCE ""AccountingPeriod_Id_seq"" RESTART WITH 1;
                        ELSE
                            PERFORM setval(
                                pg_get_serial_sequence('public.""AccountingPeriod""', 'Id'),
                                (SELECT MAX(""Id"") FROM ""AccountingPeriod""));    
                        END IF;

                        -- Tenant
                        IF ( SELECT COALESCE(MAX(""Id""), 0) FROM ""Tenant"" ) = 0 THEN
                            ALTER SEQUENCE ""Tenant_Id_seq"" RESTART WITH 1;
                        ELSE
                            PERFORM setval(
                                pg_get_serial_sequence('public.""Tenant""', 'Id'),
                                (SELECT MAX(""Id"") FROM ""Tenant""));    
                        END IF;
                    END $$ LANGUAGE plpgsql;
                    ");
            }
        }

        private IAccountingPeriodRepository GetAccountingPeriodRepository()
        {
            var config = TestUtilities.GetConfiguration();
            var appDbContext = TestUtilities.GetDatabaseContextAsync().Result;
            return new AccountingPeriodRepository(appDbContext);
        }
    }
}
