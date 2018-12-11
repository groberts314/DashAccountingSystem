using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Data.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _db = null;

        public AccountRepository(ApplicationDbContext applicationDbContext)
        {
            _db = applicationDbContext;
        }

        public async Task<IEnumerable<Account>> GetAccountsByTenantAsync(int tenantId)
        {
            return await _db
                .Account
                .Include("AccountType")
                .Include("AssetType")
                .Where(a => a.TenantId == tenantId)
                .OrderBy(a => a.AccountTypeId)
                .ThenBy(a => a.AccountNumber)
                .ToListAsync();
        }
    }
}
