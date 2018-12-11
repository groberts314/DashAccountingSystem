using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Dapper;
using Npgsql;
using Xunit;
using DashAccountingSystem.Data.Models;
using DashAccountingSystem.Data.Repositories;

namespace DashAccountingSystem.Tests
{
    public class AccountingPeriodRepositoryFixture
    {
        private int _tenantId = 0;
        private List<int> _addedAccountingPeriodIds = new List<int>();

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
                    var accountingPeriod = repository.FetchOrCreateAccountPeriodAsync(_tenantId, year, month).Result;
                    Assert.NotNull(accountingPeriod);
                    Assert.True(accountingPeriod.Id > 0);
                    _addedAccountingPeriodIds.Add(accountingPeriod.Id);
                    Assert.Equal(_tenantId, accountingPeriod.TenantId);
                    Assert.Equal(year, accountingPeriod.Year);
                    Assert.Equal(month, accountingPeriod.Month);

                    var fetchedAccountingPeriod = repository.FetchOrCreateAccountPeriodAsync(_tenantId, year, month).Result;
                    Assert.Equal(accountingPeriod.Id, fetchedAccountingPeriod.Id);
                    Assert.Equal(_tenantId, fetchedAccountingPeriod.TenantId);
                    Assert.Equal(year, fetchedAccountingPeriod.Year);
                    Assert.Equal(month, fetchedAccountingPeriod.Month);
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
                connection.Execute(@"
                    DELETE FROM ""AccountingPeriod"" WHERE ""Id"" = ANY ( @_addedAccountingPeriodIds );",
                    new { _addedAccountingPeriodIds });

                connection.Execute(@"
                    DELETE FROM ""Tenant"" WHERE ""Id"" = @_tenantId;",
                    new { _tenantId });

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

                _addedAccountingPeriodIds.Clear();
            }
        }

        private IAccountingPeriodRepository GetAccountingPeriodRepository()
        {
            var config = TestUtilities.GetConfiguration();
            var appDbContext = TestUtilities.GetDatabaseContext().Result;
            return new AccountingPeriodRepository(appDbContext);
        }
    }
}
