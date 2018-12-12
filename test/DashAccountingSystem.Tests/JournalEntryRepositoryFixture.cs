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

        [Fact]
        [Trait("Category", "Requires Database")]
        public void InsertJournalEntry_Ok()
        {
            lock (TestUtilities.DatabaseSyncLock)
            {
                try
                {
                    // ARRANGE
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

                    var journalEntry = new JournalEntry(
                        _tenantId,
                        entryDate,
                        null,
                        "Payment for Invoice #1001",
                        null,
                        _userId,
                        null);

                    var transactionAmount = 10000.00m;

                    journalEntry.Accounts.Add(new JournalEntryAccount(
                        cashAccount.Id, transactionAmount, assetTypeUSD.Id));
                    journalEntry.Accounts.Add(new JournalEntryAccount(
                        revenueAccount.Id, -transactionAmount, assetTypeUSD.Id));

                    var accountingPeriodRepo = GetAccountingPeriodRepository();
                    var journalEntryRepo = GetJournalEntryRepository(accountingPeriodRepo);

                    // ACT - INSERT THE JOURNAL ENTRY
                    var savedJournalEntry = journalEntryRepo.CreateJournalEntryAsync(journalEntry).Result;

                    // ASSERT all the things!
                    Assert.NotNull(savedJournalEntry);
                    Assert.True(savedJournalEntry.Id > 0);
                    Assert.True(savedJournalEntry.EntryId > 0);
                    Assert.NotNull(savedJournalEntry.AccountingPeriod);
                    Assert.True(savedJournalEntry.AccountingPeriod.ContainsDate(entryDate));

                    Assert.Equal(2, savedJournalEntry.Accounts.Count());
                    Assert.All(savedJournalEntry.Accounts, jeAcct => Assert.NotNull(jeAcct?.Account));

                    Assert.Contains(savedJournalEntry.Accounts, jeAcct =>
                        jeAcct.AccountId == cashAccount.Id &&
                        jeAcct.Account.AccountNumber == cashAccount.AccountNumber &&
                        jeAcct.Account.Name == cashAccount.Name &&
                        jeAcct.Amount == transactionAmount);

                    Assert.Contains(savedJournalEntry.Accounts, jeAcct =>
                        jeAcct.AccountId == revenueAccount.Id &&
                        jeAcct.Account.AccountNumber == revenueAccount.AccountNumber &&
                        jeAcct.Account.Name == revenueAccount.Name &&
                        jeAcct.Amount == -transactionAmount);

                    Assert.Equal(0, savedJournalEntry.Accounts.Sum(jeAcct => jeAcct.Amount));
                    Assert.Equal(0, savedJournalEntry.Accounts.Sum(jeAcct => jeAcct.Amount));
                    Assert.True(savedJournalEntry.IsBalanced);
                    Assert.True(savedJournalEntry.IsPending);
                }
                finally
                {
                    Cleanup();
                }
            }
        }

        [Fact]
        [Trait("Category", "Requires Database")]
        public void InsertPostedJournalEntry_Ok()
        {
            lock (TestUtilities.DatabaseSyncLock)
            {
                try
                {
                    // ARRANGE - SET UP A JOURNAL ENTRY THAT INCLUDES A POST DATE
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
                    var postDate = entryDate.AddDays(3);

                    var journalEntry = new JournalEntry(
                        _tenantId,
                        entryDate,
                        postDate,
                        "Payment for Invoice #1001",
                        null,
                        _userId,
                        _userId);

                    var transactionAmount = 10000.00m;

                    journalEntry.Accounts.Add(
                        new JournalEntryAccount(cashAccount.Id, transactionAmount, assetTypeUSD.Id));
                    journalEntry.Accounts.Add(
                        new JournalEntryAccount(revenueAccount.Id, -transactionAmount, assetTypeUSD.Id));

                    var accountingPeriodRepo = GetAccountingPeriodRepository();
                    var journalEntryRepo = GetJournalEntryRepository(accountingPeriodRepo);

                    // ACT - CREATE THE POSTED JOURNAL ENTRY
                    var savedJournalEntry = journalEntryRepo.CreateJournalEntryAsync(journalEntry).Result;

                    // ASSERT all the things!
                    // Including all the things about Posted Entries and affected Accounts!
                    Assert.NotNull(savedJournalEntry);
                    Assert.True(savedJournalEntry.Id > 0);
                    Assert.True(savedJournalEntry.EntryId > 0);
                    Assert.NotNull(savedJournalEntry.AccountingPeriod);
                    Assert.True(savedJournalEntry.AccountingPeriod.ContainsDate(entryDate));

                    Assert.Equal(2, savedJournalEntry.Accounts.Count());
                    Assert.All(savedJournalEntry.Accounts, jeAcct => Assert.NotNull(jeAcct?.Account));

                    Assert.Contains(savedJournalEntry.Accounts, jeAcct =>
                        jeAcct.AccountId == cashAccount.Id &&
                        jeAcct.Account.AccountNumber == cashAccount.AccountNumber &&
                        jeAcct.Account.Name == cashAccount.Name &&
                        jeAcct.Amount == transactionAmount);

                    Assert.Contains(savedJournalEntry.Accounts, jeAcct =>
                        jeAcct.AccountId == revenueAccount.Id &&
                        jeAcct.Account.AccountNumber == revenueAccount.AccountNumber &&
                        jeAcct.Account.Name == revenueAccount.Name &&
                        jeAcct.Amount == -transactionAmount);

                    Assert.Equal(0, savedJournalEntry.Accounts.Sum(jeAcct => jeAcct.Amount));
                    Assert.True(savedJournalEntry.IsBalanced);
                    Assert.False(savedJournalEntry.IsPending);
                    Assert.NotNull(savedJournalEntry.PostDate);
                    Assert.Equal(postDate, savedJournalEntry.PostDate);

                    var updatedCashJournalEntryAccount = savedJournalEntry
                        .Accounts
                        .FirstOrDefault(jea => jea.AccountId == cashAccount.Id);

                    Assert.NotNull(updatedCashJournalEntryAccount);
                    Assert.NotNull(updatedCashJournalEntryAccount.Account);
                    Assert.NotNull(updatedCashJournalEntryAccount.PreviousBalance);
                    Assert.Equal(0.0m, updatedCashJournalEntryAccount.PreviousBalance);
                    Assert.NotNull(updatedCashJournalEntryAccount.NewBalance);
                    Assert.Equal(transactionAmount, updatedCashJournalEntryAccount.NewBalance);
                    Assert.Equal(transactionAmount, updatedCashJournalEntryAccount.Account.CurrentBalance);
                    Assert.True(updatedCashJournalEntryAccount.Account.IsBalanceNormal);
                    Assert.True(updatedCashJournalEntryAccount.Account.BalanceUpdated > cashAccount.BalanceUpdated);

                    var updatedRevenueJournalEntryAccount = savedJournalEntry
                        .Accounts
                        .FirstOrDefault(jea => jea.AccountId == revenueAccount.Id);

                    Assert.NotNull(updatedRevenueJournalEntryAccount);
                    Assert.NotNull(updatedRevenueJournalEntryAccount.Account);
                    Assert.NotNull(updatedRevenueJournalEntryAccount.PreviousBalance);
                    Assert.Equal(0.0m, updatedRevenueJournalEntryAccount.PreviousBalance);
                    Assert.NotNull(updatedRevenueJournalEntryAccount.NewBalance);
                    Assert.Equal(-transactionAmount, updatedRevenueJournalEntryAccount.NewBalance);
                    Assert.Equal(-transactionAmount, updatedRevenueJournalEntryAccount.Account.CurrentBalance);
                    Assert.True(updatedRevenueJournalEntryAccount.Account.IsBalanceNormal);
                    Assert.True(updatedRevenueJournalEntryAccount.Account.BalanceUpdated > cashAccount.BalanceUpdated);

                }
                finally
                {
                    Cleanup();
                }
            }
        }

        [Fact]
        [Trait("Category", "Requires Database")]
        public void PostJournalEntry_Ok()
        {
            lock (TestUtilities.DatabaseSyncLock)
            {
                try
                {
                    // ARRANGE - SET UP A PENDING JOURNAL ENTRY
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

                    var journalEntry = new JournalEntry(
                        _tenantId,
                        entryDate,
                        null,
                        "Payment for Invoice #1001",
                        null,
                        _userId,
                        null);

                    var transactionAmount = 10000.00m;

                    journalEntry.Accounts.Add(
                        new JournalEntryAccount(cashAccount.Id, transactionAmount, assetTypeUSD.Id));
                    journalEntry.Accounts.Add(
                        new JournalEntryAccount(revenueAccount.Id, -transactionAmount, assetTypeUSD.Id));

                    var accountingPeriodRepo = GetAccountingPeriodRepository();
                    var journalEntryRepo = GetJournalEntryRepository(accountingPeriodRepo);

                    var savedJournalEntry = journalEntryRepo.CreateJournalEntryAsync(journalEntry).Result;

                    Assert.NotNull(savedJournalEntry);
                    Assert.True(savedJournalEntry.Id > 0);
                    Assert.True(savedJournalEntry.EntryId > 0);
                    Assert.NotNull(savedJournalEntry.AccountingPeriod);
                    Assert.True(savedJournalEntry.AccountingPeriod.ContainsDate(entryDate));

                    Assert.Equal(2, savedJournalEntry.Accounts.Count());
                    Assert.All(savedJournalEntry.Accounts, jeAcct => Assert.NotNull(jeAcct?.Account));

                    Assert.Contains(savedJournalEntry.Accounts, jeAcct =>
                        jeAcct.AccountId == cashAccount.Id &&
                        jeAcct.Account.AccountNumber == cashAccount.AccountNumber &&
                        jeAcct.Account.Name == cashAccount.Name &&
                        jeAcct.Amount == transactionAmount);

                    Assert.Contains(savedJournalEntry.Accounts, jeAcct =>
                        jeAcct.AccountId == revenueAccount.Id &&
                        jeAcct.Account.AccountNumber == revenueAccount.AccountNumber &&
                        jeAcct.Account.Name == revenueAccount.Name &&
                        jeAcct.Amount == -transactionAmount);

                    Assert.Equal(0, savedJournalEntry.Accounts.Sum(jeAcct => jeAcct.Amount));
                    Assert.True(savedJournalEntry.IsBalanced);
                    Assert.True(savedJournalEntry.IsPending);

                    // ACT - POST THE ENTRY
                    var postDate = entryDate.AddDays(2);
                    var postedJournalEntry = journalEntryRepo.PostJournalEntryAsync(
                        savedJournalEntry.Id,
                        postDate,
                        _userId)
                        .Result;

                    // ASSERT All the Things!
                    Assert.NotNull(postedJournalEntry);
                    Assert.NotNull(postedJournalEntry.PostDate);
                    Assert.Equal(postDate, postedJournalEntry.PostDate);
                    Assert.False(postedJournalEntry.IsPending);

                    var updatedCashJournalEntryAccount = postedJournalEntry
                        .Accounts
                        .FirstOrDefault(jea => jea.AccountId == cashAccount.Id);

                    Assert.NotNull(updatedCashJournalEntryAccount);
                    Assert.NotNull(updatedCashJournalEntryAccount.Account);
                    Assert.NotNull(updatedCashJournalEntryAccount.PreviousBalance);
                    Assert.Equal(0.0m, updatedCashJournalEntryAccount.PreviousBalance);
                    Assert.NotNull(updatedCashJournalEntryAccount.NewBalance);
                    Assert.Equal(transactionAmount, updatedCashJournalEntryAccount.NewBalance);
                    Assert.Equal(transactionAmount, updatedCashJournalEntryAccount.Account.CurrentBalance);
                    Assert.True(updatedCashJournalEntryAccount.Account.IsBalanceNormal);
                    Assert.True(updatedCashJournalEntryAccount.Account.BalanceUpdated > cashAccount.BalanceUpdated);

                    var updatedRevenueJournalEntryAccount = postedJournalEntry
                        .Accounts
                        .FirstOrDefault(jea => jea.AccountId == revenueAccount.Id);

                    Assert.NotNull(updatedRevenueJournalEntryAccount);
                    Assert.NotNull(updatedRevenueJournalEntryAccount.Account);
                    Assert.NotNull(updatedRevenueJournalEntryAccount.PreviousBalance);
                    Assert.Equal(0.0m, updatedRevenueJournalEntryAccount.PreviousBalance);
                    Assert.NotNull(updatedRevenueJournalEntryAccount.NewBalance);
                    Assert.Equal(-transactionAmount, updatedRevenueJournalEntryAccount.NewBalance);
                    Assert.Equal(-transactionAmount, updatedRevenueJournalEntryAccount.Account.CurrentBalance);
                    Assert.True(updatedRevenueJournalEntryAccount.Account.IsBalanceNormal);
                    Assert.True(updatedRevenueJournalEntryAccount.Account.BalanceUpdated > cashAccount.BalanceUpdated);

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
                var parameters = new { _tenantId };

                connection.Execute(@"
                    DELETE FROM ""JournalEntryAccount""
                    WHERE ""JournalEntryId"" IN (
                        SELECT ""Id"" FROM ""JournalEntry"" WHERE ""TenantId"" = @_tenantId
                    );",
                    parameters);

                connection.Execute(@"
                    DELETE FROM ""JournalEntry"" WHERE ""TenantId"" = @_tenantId;",
                    parameters);

                connection.Execute(@"
                    DELETE FROM ""Account"" WHERE ""TenantId"" = @_tenantId;",
                    parameters);

                connection.Execute(@"
                    DELETE FROM ""AccountingPeriod"" WHERE ""TenantId"" = @_tenantId;",
                    parameters);

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
            }
        }
    }
}
