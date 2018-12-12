using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Data.Repositories
{
    public class AccountingPeriodRepository : IAccountingPeriodRepository
    {
        private readonly ApplicationDbContext _db = null;

        public AccountingPeriodRepository(ApplicationDbContext applicationDbContext)
        {
            _db = applicationDbContext;
        }

        public async Task<AccountingPeriod> FetchOrCreateAccountPeriodAsync(int tenantId, DateTime date)
        {
            return await FetchOrCreateAccountPeriodAsync(tenantId, date.Year, (byte)date.Month);
        }

        public async Task<AccountingPeriod> FetchOrCreateAccountPeriodAsync(int tenantId, int year, byte month)
        {
            var existingAccountingPeriod = await _db.AccountingPeriod
                .FirstOrDefaultAsync(ap => ap.TenantId == tenantId && ap.Year == year && ap.Month == month);

            if (existingAccountingPeriod != null)
                return existingAccountingPeriod;

            var newAccountingPeriod = new AccountingPeriod(tenantId, year, month);
            await _db.AccountingPeriod.AddAsync(newAccountingPeriod);
            await _db.SaveChangesAsync();

            return newAccountingPeriod;
        }
    }
}
