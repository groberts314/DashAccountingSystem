using System;
using System.Threading.Tasks;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Data.Repositories
{
    public interface IAccountingPeriodRepository
    {
        Task<AccountingPeriod> FetchOrCreateAccountingPeriodAsync(
            int tenantId,
            AccountingPeriodType periodType,
            DateTime date);

        Task<AccountingPeriod> FetchOrCreateMonthlyAccountingPeriodAsync(
            int tenantId,
            int year,
            byte month);

        Task<AccountingPeriod> FetchOrCreateQuarterlyAccountingPeriodAsync(
            int tenantId,
            int year,
            byte quarter);

        Task<AccountingPeriod> FetchOrCreateYearlyAccountingPeriodAsync(
            int tenantId,
            int year);
    }
}
