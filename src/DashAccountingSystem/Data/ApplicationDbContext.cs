using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<AccountingPeriod> AccountingPeriod { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<AccountType> AccountType { get; set; }
        public DbSet<AssetType> AssetType { get; set; }
        public DbSet<JournalEntry> JournalEntry { get; set; }
        public DbSet<JournalEntryAccount> JournalEntryAccount { get; set; }
        public DbSet<Tenant> Tenant { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Schema (Indexes and Such)
            builder.Entity<Tenant>()
                .HasIndex(t => t.Name)
                .IsUnique();

            builder.Entity<AccountType>()
                .HasIndex(at => at.Name)
                .IsUnique();

            builder.Entity<AssetType>()
                .HasIndex(at => at.Name)
                .IsUnique();

            builder.Entity<AccountingPeriod>()
                .HasIndex(ap => new { ap.TenantId, ap.Name })
                .IsUnique();

            builder.Entity<AccountingPeriod>()
                .HasIndex(ap => new { ap.TenantId, ap.Year, ap.Month })
                .IsUnique();

            builder.Entity<AccountingPeriod>()
                .HasIndex(ap => new { ap.TenantId, ap.Year });

            builder.Entity<AccountingPeriod>()
                .HasIndex(ap => new { ap.TenantId, ap.Year, ap.Quarter });

            builder.Entity<Account>()
                .HasIndex(a => new { a.TenantId, a.AccountNumber })
                .IsUnique();

            builder.Entity<Account>()
                .HasIndex(a => new { a.TenantId, a.Name })
                .IsUnique();

            builder.Entity<Account>()
                .Property("Created")
                .HasColumnType("TIMESTAMP")
                .HasDefaultValueSql("now() AT TIME ZONE 'UTC'");

            builder.Entity<JournalEntry>()
                .HasIndex(je => new { je.TenantId, je.EntryId })
                .IsUnique();

            builder.Entity<JournalEntry>()
                .Property("Created")
                .HasColumnType("TIMESTAMP")
                .HasDefaultValueSql("now() AT TIME ZONE 'UTC'");

            // Seed Data
            // FIXME: This isn't working.  EF seems stupid about identity/auto-generated columns no matter what you do. :-(
            /*
            builder.Entity<AccountType>()
                .HasData(
                    new AccountType("Asset"),
                    new AccountType("Liability"),
                    new AccountType("Equity"),
                    new AccountType("Revenue"),
                    new AccountType("Expense")
                );

            builder.Entity<AssetType>()
                .HasData(
                    new AssetType("USD $")
                );

            builder.Entity<Tenant>()
                .HasData(
                    new Tenant("Example Corporation"),
                    new Tenant("Dash Software Solutions, Inc.")
                );
            */
        }
    }
}
