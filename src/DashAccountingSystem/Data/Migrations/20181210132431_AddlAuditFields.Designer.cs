﻿// <auto-generated />
using System;
using DashAccountingSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DashAccountingSystem.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20181210132431_AddlAuditFields")]
    partial class AddlAuditFields
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("DashAccountingSystem.Data.Models.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccountNumber");

                    b.Property<int>("AccountTypeId");

                    b.Property<int>("AssetTypeId");

                    b.Property<DateTime>("BalanceUpdated");

                    b.Property<DateTime>("Created")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("TIMESTAMP")
                        .HasDefaultValueSql("now() AT TIME ZONE 'UTC'");

                    b.Property<string>("CreatedById")
                        .IsRequired();

                    b.Property<decimal>("CurrentBalance");

                    b.Property<string>("Description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<short>("NormalBalanceType");

                    b.Property<int>("TenantId");

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedById");

                    b.HasKey("Id");

                    b.HasIndex("AccountTypeId");

                    b.HasIndex("AssetTypeId");

                    b.HasIndex("CreatedById");

                    b.HasIndex("UpdatedById");

                    b.HasIndex("TenantId", "AccountNumber")
                        .IsUnique();

                    b.HasIndex("TenantId", "Name")
                        .IsUnique();

                    b.ToTable("Account");
                });

            modelBuilder.Entity("DashAccountingSystem.Data.Models.AccountingPeriod", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte>("Month");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<byte>("Quarter");

                    b.Property<int>("TenantId");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.HasIndex("TenantId", "Name")
                        .IsUnique();

                    b.HasIndex("TenantId", "Year");

                    b.HasIndex("TenantId", "Year", "Month")
                        .IsUnique();

                    b.HasIndex("TenantId", "Year", "Quarter");

                    b.ToTable("AccountingPeriod");
                });

            modelBuilder.Entity("DashAccountingSystem.Data.Models.AccountType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("AccountType");
                });

            modelBuilder.Entity("DashAccountingSystem.Data.Models.AssetType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("AssetType");
                });

            modelBuilder.Entity("DashAccountingSystem.Data.Models.JournalEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccountingPeriodId");

                    b.Property<int?>("CheckNumber");

                    b.Property<DateTime>("Created")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("TIMESTAMP")
                        .HasDefaultValueSql("now() AT TIME ZONE 'UTC'");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(2048);

                    b.Property<string>("EnteredById")
                        .IsRequired();

                    b.Property<DateTime>("EntryDate");

                    b.Property<int>("EntryId");

                    b.Property<DateTime?>("PostDate");

                    b.Property<string>("PostedById");

                    b.Property<int>("TenantId");

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedById");

                    b.HasKey("Id");

                    b.HasIndex("AccountingPeriodId");

                    b.HasIndex("EnteredById");

                    b.HasIndex("PostedById");

                    b.HasIndex("UpdatedById");

                    b.HasIndex("TenantId", "EntryId")
                        .IsUnique();

                    b.ToTable("JournalEntry");
                });

            modelBuilder.Entity("DashAccountingSystem.Data.Models.JournalEntryAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccountId");

                    b.Property<decimal>("Amount");

                    b.Property<int>("AssetTypeId");

                    b.Property<int>("JournalEntryId");

                    b.Property<decimal?>("NewBalance");

                    b.Property<decimal?>("PreviousBalance");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("AssetTypeId");

                    b.HasIndex("JournalEntryId");

                    b.ToTable("JournalEntryAccount");
                });

            modelBuilder.Entity("DashAccountingSystem.Data.Models.Tenant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Tenant");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128);

                    b.Property<string>("Name")
                        .HasMaxLength(128);

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("DashAccountingSystem.Data.Models.Account", b =>
                {
                    b.HasOne("DashAccountingSystem.Data.Models.AccountType", "AccountType")
                        .WithMany()
                        .HasForeignKey("AccountTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DashAccountingSystem.Data.Models.AssetType", "AssetType")
                        .WithMany()
                        .HasForeignKey("AssetTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DashAccountingSystem.Data.Models.Tenant", "Tenant")
                        .WithMany("Accounts")
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", "UpdatedBy")
                        .WithMany()
                        .HasForeignKey("UpdatedById");
                });

            modelBuilder.Entity("DashAccountingSystem.Data.Models.AccountingPeriod", b =>
                {
                    b.HasOne("DashAccountingSystem.Data.Models.Tenant")
                        .WithMany("AccountingPeriods")
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DashAccountingSystem.Data.Models.JournalEntry", b =>
                {
                    b.HasOne("DashAccountingSystem.Data.Models.AccountingPeriod", "AccountingPeriod")
                        .WithMany()
                        .HasForeignKey("AccountingPeriodId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", "EnteredBy")
                        .WithMany()
                        .HasForeignKey("EnteredById")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", "PostedBy")
                        .WithMany()
                        .HasForeignKey("PostedById");

                    b.HasOne("DashAccountingSystem.Data.Models.Tenant", "Tenant")
                        .WithMany("JournalEntries")
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", "UpdatedBy")
                        .WithMany()
                        .HasForeignKey("UpdatedById");
                });

            modelBuilder.Entity("DashAccountingSystem.Data.Models.JournalEntryAccount", b =>
                {
                    b.HasOne("DashAccountingSystem.Data.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DashAccountingSystem.Data.Models.AssetType", "AssetType")
                        .WithMany()
                        .HasForeignKey("AssetTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DashAccountingSystem.Data.Models.JournalEntry", "JournalEntry")
                        .WithMany("Accounts")
                        .HasForeignKey("JournalEntryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}