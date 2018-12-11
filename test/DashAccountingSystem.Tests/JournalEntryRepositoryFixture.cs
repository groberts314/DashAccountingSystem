using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Dapper;
using Npgsql;
using Xunit;
using DashAccountingSystem.Data.Models;
using DashAccountingSystem.Data.Repositories;

namespace DashAccountingSystem.Tests
{
    public class JournalEntryRepositoryFixture
    {
        private int _tenantId = 0;
        private Guid _userId;
        private List<int> _addedAccountingPeriodIds = new List<int>();
        private List<int> _addedAccountIds = new List<int>();
        private List<int> _addedJournalEntryIds = new List<int>();

        [Fact]
        [Trait("Category", "Requires Database")]
        public void InsertJournalEntry_Ok()
        {
            lock (TestUtilities.DatabaseSyncLock)
            {
                try
                {
                    InitializeTenant();
                    InitializeUser();

                    var sharedLookupRepo = GetSharedLookupRepository();

                    var accountTypeAsset = sharedLookupRepo
                        .GetAccountTypesAsync()
                        .Result
                        .FirstOrDefault(at => at.Name == "Asset");

                    var assetTypeUSD = sharedLookupRepo
                        .GetAssetTypesAsync()
                        .Result
                        .FirstOrDefault(at => at.Name == "USD $");

                    var cashAccount = MakeAccount(
                        1010, "Operating Cash Account", accountTypeAsset, assetTypeUSD, BalanceType.Debit);

                    var accountTypeRevenue = sharedLookupRepo
                        .GetAccountTypesAsync()
                        .Result
                        .FirstOrDefault(at => at.Name == "Revenue");

                    var revenueAccount = MakeAccount(
                        4010, "Payments for Services Rendered", accountTypeRevenue, assetTypeUSD, BalanceType.Credit);

                    var entryDate = new DateTime(2018, 12, 11, 0, 0, 0, DateTimeKind.Utc);
                    var accountingPeriodRepo = GetAccountingPeriodRepository();
                    var accountingPeriod = accountingPeriodRepo.FetchOrCreateAccountPeriodAsync(_tenantId, 2018, 12).Result;

                    Assert.NotNull(accountingPeriod);
                    Assert.True(accountingPeriod.Id > 0);
                    _addedAccountingPeriodIds.Add(accountingPeriod.Id);

                    var journalEntry = new JournalEntry(
                        _tenantId,
                        1,
                        accountingPeriod.Id,
                        entryDate,
                        null,
                        "Payment for Invoice #1001",
                        null,
                        _userId,
                        null);

                    var transactionAmount = 10000.00m;

                    journalEntry.Accounts.Add(new JournalEntryAccount(cashAccount.Id, transactionAmount, assetTypeUSD.Id));
                    journalEntry.Accounts.Add(new JournalEntryAccount(revenueAccount.Id, -transactionAmount, assetTypeUSD.Id));

                    var journalEntryRepo = GetJournalEntryRepository(accountingPeriodRepo);
                    var savedJournalEntry = journalEntryRepo.InsertJournalEntryAsync(journalEntry).Result;

                    Assert.NotNull(savedJournalEntry);
                    Assert.True(savedJournalEntry.Id > 0);
                    _addedJournalEntryIds.Add(savedJournalEntry.Id);

                    // TODO: More robust assertions
                }
                finally
                {
                    Cleanup();
                }
            }
        }

        private Account MakeAccount(
            ushort accountNumber,
            string name,
            AccountType accountType,
            AssetType assetType,
            BalanceType balanceType)
        {
            var account = new Account(
                _tenantId,
                accountNumber,
                name,
                null,
                accountType.Id,
                assetType.Id,
                balanceType,
                _userId);

            var accountRepo = GetAccountRepository();
            var savedAccount = accountRepo.CreateAccountAsync(account).Result;

            Assert.NotNull(savedAccount);
            Assert.True(savedAccount.Id > 0);
            _addedAccountIds.Add(savedAccount.Id);

            return savedAccount;
        }

        private IAccountRepository GetAccountRepository()
        {
            var appDbContext = TestUtilities.GetDatabaseContextAsync().Result;
            return new AccountRepository(appDbContext);
        }

        private IAccountingPeriodRepository GetAccountingPeriodRepository()
        {
            var appDbContext = TestUtilities.GetDatabaseContextAsync().Result;
            return new AccountingPeriodRepository(appDbContext);
        }

        private IJournalEntryRepository GetJournalEntryRepository(IAccountingPeriodRepository accountingPeriodRepository)
        {
            var appDbContext = TestUtilities.GetDatabaseContextAsync().Result;
            return new JournalEntryRepository(appDbContext, accountingPeriodRepository);
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
                connection.Execute(@"
                    DELETE FROM ""JournalEntryAccount"" WHERE ""JournalEntryId"" = ANY ( @_addedJournalEntryIds );",
                    new { _addedJournalEntryIds });

                connection.Execute(@"
                    DELETE FROM ""JournalEntry"" WHERE ""Id"" = ANY ( @_addedJournalEntryIds );",
                    new { _addedJournalEntryIds });

                connection.Execute(@"
                    DELETE FROM ""Account"" WHERE ""Id"" = ANY ( @_addedAccountIds );",
                    new { _addedAccountIds });

                connection.Execute(@"
                    DELETE FROM ""AccountingPeriod"" WHERE ""Id"" = ANY ( @_addedAccountingPeriodIds );",
                    new { _addedAccountingPeriodIds });

                connection.Execute(@"
                    DELETE FROM ""Tenant"" WHERE ""Id"" = @_tenantId;",
                    new { _tenantId });

                connection.Execute(@"
                    DO $$
                    BEGIN
                        -- JournalEntryAccount
                        IF ( SELECT COALESCE(MAX(""Id""), 0) FROM ""JournalEntryAccount"" ) = 0 THEN
                            ALTER SEQUENCE ""JournalEntryAccount_Id_seq"" RESTART WITH 1;
                        ELSE
                            PERFORM setval(
                                pg_get_serial_sequence('public.""JournalEntryAccount""', 'Id'),
                                (SELECT MAX(""Id"") FROM ""JournalEntryAccount""));    
                        END IF;

                        -- JournalEntry
                        IF ( SELECT COALESCE(MAX(""Id""), 0) FROM ""JournalEntry"" ) = 0 THEN
                            ALTER SEQUENCE ""JournalEntry_Id_seq"" RESTART WITH 1;
                        ELSE
                            PERFORM setval(
                                pg_get_serial_sequence('public.""JournalEntry""', 'Id'),
                                (SELECT MAX(""Id"") FROM ""JournalEntry""));    
                        END IF;

                        -- Account
                        IF ( SELECT COALESCE(MAX(""Id""), 0) FROM ""Account"" ) = 0 THEN
                            ALTER SEQUENCE ""Account_Id_seq"" RESTART WITH 1;
                        ELSE
                            PERFORM setval(
                                pg_get_serial_sequence('public.""Account""', 'Id'),
                                (SELECT MAX(""Id"") FROM ""Account""));    
                        END IF;

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

                _addedAccountIds.Clear();
            }
        }
    }
}
