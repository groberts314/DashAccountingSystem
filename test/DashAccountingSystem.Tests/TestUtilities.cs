using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using DashAccountingSystem.Data;
using Npgsql;

namespace DashAccountingSystem.Tests
{
    public static class TestUtilities
    {
        internal static object DatabaseSyncLock = new object();

        private static Lazy<IConfiguration> _configuration = new Lazy<IConfiguration>(LoadConfiguration);
        private static Lazy<string> _connectionString = new Lazy<string>(InitializeConnectionString);

        private static IConfiguration LoadConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("config.json", optional: false)
                .AddUserSecrets<SharedLookupRepositoryFixture>() // any non-static class in this project will do
                .Build();
        }

        private static string InitializeConnectionString()
        {
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder(
                _configuration.Value.GetConnectionString("DefaultConnection"));

            connectionStringBuilder.Password = _configuration.Value["DbPassword"];

            return connectionStringBuilder.ConnectionString;
        }

        internal static IConfiguration GetConfiguration()
        {
            return _configuration.Value;
        }

        internal static string GetConnectionString()
        {
            return _connectionString.Value;
        }

        internal static async Task<ApplicationDbContext> GetDatabaseContext()
        {
            var connectionString = GetConnectionString();

            DbContextOptions<ApplicationDbContext> options;
            var dbCtxOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            dbCtxOptionsBuilder.UseNpgsql(connectionString);

            options = dbCtxOptionsBuilder.Options;

            ApplicationDbContext appDbContext = new ApplicationDbContext(options);
            await appDbContext.Database.EnsureCreatedAsync();
            await appDbContext.Database.MigrateAsync();
            return appDbContext;
        }
    }
}
