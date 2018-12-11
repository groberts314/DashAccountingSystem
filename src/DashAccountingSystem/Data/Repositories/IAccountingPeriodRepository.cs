using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Data.Repositories
{
    public interface IAccountingPeriodRepository
    {
        Task<AccountingPeriod> FetchOrCreateAccountPeriodAsync(int tenantId, int year, byte month);
    }
}
