using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Data.Repositories
{
    public class SharedLookupRepository : ISharedLookupRepository
    {
        private readonly ApplicationDbContext _db = null;

        public SharedLookupRepository(ApplicationDbContext applicationDbContext)
        {
            _db = applicationDbContext;
        }

        public async Task<IEnumerable<AccountType>> GetAccountTypesAsync()
        {
            return await _db.AccountType.ToListAsync();
        }

        public async Task<IEnumerable<AssetType>> GetAssetTypesAsync()
        {
            return await _db.AssetType.ToListAsync();
        }
    }
}
