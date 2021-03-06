﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using DashAccountingSystem.Data;
using DashAccountingSystem.Data.Models;
using DashAccountingSystem.Data.Repositories;
using DashAccountingSystem.Extensions;
using DashAccountingSystem.Security.Authentication;
using DashAccountingSystem.Security.Authorization;

namespace DashAccountingSystem
{
    public class Startup
    {
        private readonly ILogger _logger;
        private string _connectionString = null;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Build Connection String by including DbPassword from User Secrets
            var builder = new NpgsqlConnectionStringBuilder(
                Configuration.GetConnectionString("DefaultConnection"));
            builder.Password = Configuration["DbPassword"];
            _connectionString = builder.ConnectionString;

            if (string.IsNullOrEmpty(_connectionString))
            {
                _logger.LogCritical("Cannot start application server without connection string.");
                throw new Exception("No connection string.");
            }

            _logger.LogInformation("Using connection string: '{0}'", _connectionString.MaskPassword());

            services.AddDbContext<ApplicationDbContext>(options =>
                // SQL Server
                //options.UseSqlServer(
                //    Configuration.GetConnectionString("DefaultConnection")));
                // PostgreSQL
                options.UseNpgsql(_connectionString));

            services
                .AddDefaultIdentity<ApplicationUser>()
                .AddRoles<ApplicationRole>()
                .AddRoleManager<ApplicationRoleManager>()
                .AddUserManager<ApplicationUserManager>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationClaimsPrincipalFactory>();

            services
                .AddScoped<IAccountingPeriodRepository, AccountingPeriodRepository>()
                .AddScoped<IAccountRepository, AccountRepository>()
                .AddScoped<IJournalEntryRepository, JournalEntryRepository>()
                .AddScoped<ISharedLookupRepository, SharedLookupRepository>()
                .AddScoped<ITenantRepository, TenantRepository>();

            services.AddMvc(config =>
            {
                var requireAuthenticatedUsersPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                config.Filters.Add(new AuthorizeFilter(requireAuthenticatedUsersPolicy));
            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
