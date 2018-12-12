using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Dapper;
using Npgsql;
using Xunit;
using DashAccountingSystem.Data.Models;
using DashAccountingSystem.Data.Repositories;

namespace DashAccountingSystem.Tests
{
    public class AccountRepositoryFixture
    {
        private Guid _userId;
        private int _tenantId = 0;

        [Fact]
        [Trait("Category", "Requires Database")]
        public void CreateAccount_Ok()
        {
            lock (TestUtilities.DatabaseSyncLock)
            {
                try
                {
                    InitializeTenant();
                    InitializeUser();

                    var sharedLookupRepo = GetSharedLookupRepository();
                    var accountRepo = GetAccountRepository();

                    var accountTypeAsset = sharedLookupRepo
                        .GetAccountTypesAsync()
                        .Result
                        .FirstOrDefault(at => at.Name == "Asset");

                    var assetTypeUSD = sharedLookupRepo
                        .GetAssetTypesAsync()
                        .Result
                        .FirstOrDefault(at => at.Name == "USD $");

                    var account = new Account(
                        _tenantId,
                        1010,
                        "Operating Cash Account",
                        "Primary business checking account.",
                        accountTypeAsset.Id,
                        assetTypeUSD.Id,
                        BalanceType.Debit,
                        _userId);

                    var savedAccount = accountRepo.CreateAccountAsync(account).Result;

                    Assert.NotNull(savedAccount);
                    Assert.True(savedAccount.Id > 0);
                }
                finally
                {
                    Cleanup();
                }
            }
        }

        private IAccountRepository GetAccountRepository()
        {
            var appDbContext = TestUtilities.GetDatabaseContextAsync().Result;
            return new AccountRepository(appDbContext);
        }

        private ISharedLookupRepository GetSharedLookupRepository()
        {
            var appDbContext = TestUtilities.GetDatabaseContextAsync().Result;
            return new SharedLookupRepository(appDbContext);
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

        private void InitializeUser()
        {
            var connString = TestUtilities.GetConnectionString();

            using (var connection = new NpgsqlConnection(connString))
            {
                _userId = connection.QueryFirstOrDefault<Guid>(@"
                    SELECT ""Id"" FROM ""AspNetUsers"" ORDER BY ""LastName"", ""FirstName"" LIMIT 1;");
            }
        }

        private void Cleanup()
        {
            var connString = TestUtilities.GetConnectionString();

            using (var connection = new NpgsqlConnection(connString))
            {
                var parameters = new { _tenantId };

                connection.Execute(@"
                    DELETE FROM ""Account"" WHERE ""TenantId"" = @_tenantId;",
                    parameters);

                connection.Execute(@"
                    DELETE FROM ""Tenant"" WHERE ""Id"" = @_tenantId;",
                    parameters);

                connection.Execute(@"
                    DO $$
                    BEGIN
                        -- Account
                        IF ( SELECT COALESCE(MAX(""Id""), 0) FROM ""Account"" ) = 0 THEN
                            ALTER SEQUENCE ""Account_Id_seq"" RESTART WITH 1;
                        ELSE
                            PERFORM setval(
                                pg_get_serial_sequence('public.""Account""', 'Id'),
                                (SELECT MAX(""Id"") FROM ""Account""));    
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
    }
}
