using System;
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

        public async Task<AccountingPeriod> FetchOrCreateAccountingPeriodAsync(
            int tenantId,
            AccountingPeriodType periodType,
            DateTime date)
        {
            switch (periodType)
            {
                case AccountingPeriodType.Month:
                    return await FetchOrCreateMonthlyAccountingPeriodAsync(tenantId, date.Year, (byte)date.Month);

                case AccountingPeriodType.Quarter:
                    var quarter = AccountingPeriod.GetQuarter((byte)date.Month);
                    return await FetchOrCreateQuarterlyAccountingPeriodAsync(tenantId, date.Year, quarter);

                case AccountingPeriodType.Year:
                    return await FetchOrCreateYearlyAccountingPeriodAsync(tenantId, date.Year);

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(periodType),
                        $"{periodType} is not a valid value for '${nameof(periodType)}'");
            }
        }

        public async Task<AccountingPeriod> FetchOrCreateMonthlyAccountingPeriodAsync(
            int tenantId,
            int year,
            byte month)
        {
            var existingAccountingPeriod = await _db.AccountingPeriod
                .FirstOrDefaultAsync(ap =>
                    ap.TenantId == tenantId &&
                    ap.PeriodType == AccountingPeriodType.Month &&
                    ap.Year == year &&
                    ap.Month == month);

            if (existingAccountingPeriod != null)
                return existingAccountingPeriod;

            var newAccountingPeriod = new AccountingPeriod(tenantId, AccountingPeriodType.Month, year, month);
            await _db.AccountingPeriod.AddAsync(newAccountingPeriod);
            await _db.SaveChangesAsync();

            return newAccountingPeriod;
        }

        public async Task<AccountingPeriod> FetchOrCreateQuarterlyAccountingPeriodAsync(
            int tenantId,
            int year,
            byte quarter)
        {
            var existingAccountingPeriod = await _db.AccountingPeriod
                .FirstOrDefaultAsync(ap =>
                    ap.TenantId == tenantId &&
                    ap.PeriodType == AccountingPeriodType.Quarter &&
                    ap.Year == year &&
                    ap.Quarter == quarter);

            if (existingAccountingPeriod != null)
                return existingAccountingPeriod;

            var newAccountingPeriod = new AccountingPeriod(
                tenantId,
                AccountingPeriodType.Quarter,
                year,
                (byte)(quarter * 3));

            await _db.AccountingPeriod.AddAsync(newAccountingPeriod);
            await _db.SaveChangesAsync();

            return newAccountingPeriod;
        }

        public async Task<AccountingPeriod> FetchOrCreateYearlyAccountingPeriodAsync(
            int tenantId,
            int year)
        {
            var existingAccountingPeriod = await _db.AccountingPeriod
                .FirstOrDefaultAsync(ap =>
                    ap.TenantId == tenantId &&
                    ap.PeriodType == AccountingPeriodType.Year);

            if (existingAccountingPeriod != null)
                return existingAccountingPeriod;

            var newAccountingPeriod = new AccountingPeriod(tenantId, AccountingPeriodType.Year, year, 12);
            await _db.AccountingPeriod.AddAsync(newAccountingPeriod);
            await _db.SaveChangesAsync();

            return newAccountingPeriod;
        }
    }
}
