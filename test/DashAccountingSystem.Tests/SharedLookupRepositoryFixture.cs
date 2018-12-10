using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;
using DashAccountingSystem.Data.Models;
using DashAccountingSystem.Data.Repositories;

namespace DashAccountingSystem.Tests
{
    public class SharedLookupRepositoryFixture
    {
        [Fact]
        [Trait("Category", "Requires Database")]
        public void GetAccountTypes_Ok()
        {
            var repository = GetSharedLookupRepository();
            var accountTypes = repository.GetAccountTypesAsync().Result;
            Assert.NotEmpty(accountTypes);
            Assert.Contains(accountTypes, at => at.Name == "Asset");
            Assert.Contains(accountTypes, at => at.Name == "Liability");
            Assert.Contains(accountTypes, at => at.Name == "Equity");
            Assert.Contains(accountTypes, at => at.Name == "Revenue");
            Assert.Contains(accountTypes, at => at.Name == "Expense");
            Assert.Contains(accountTypes, at => at.Name == "Contra");
        }

        [Fact]
        [Trait("Category", "Requires Database")]
        public void GetAssetTypes_Ok()
        {
            var repository = GetSharedLookupRepository();
            var assetTypes = repository.GetAssetTypesAsync().Result;
            Assert.NotEmpty(assetTypes);
            Assert.Contains(assetTypes, at => at.Name == "USD $");
            Assert.Contains(assetTypes, at => at.Name == "GBP £");
            Assert.Contains(assetTypes, at => at.Name == "EUR €");
            Assert.Contains(assetTypes, at => at.Name == "JPY ¥");
        }

        private ISharedLookupRepository GetSharedLookupRepository()
        {
            var config = TestUtilities.GetConfiguration<SharedLookupRepositoryFixture>();
            var appDbContext = TestUtilities.GetDatabaseContext(config).Result;
            return new SharedLookupRepository(appDbContext);
        }
    }
}
