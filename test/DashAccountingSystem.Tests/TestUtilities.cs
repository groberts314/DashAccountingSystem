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
        internal static IConfiguration GetConfiguration<TFixture>() where TFixture : class
        {
            return new ConfigurationBuilder()
                .AddJsonFile("config.json", optional: false)
                .AddUserSecrets<TFixture>()
                .Build();
        }

        internal static async Task<ApplicationDbContext> GetDatabaseContext(IConfiguration config)
        {
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder(
                config.GetConnectionString("DefaultConnection"));

            connectionStringBuilder.Password = config["DbPassword"];

            var connectionString = connectionStringBuilder.ConnectionString;

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
