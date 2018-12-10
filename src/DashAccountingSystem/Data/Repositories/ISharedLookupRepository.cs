using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Data.Repositories
{
    public interface ISharedLookupRepository
    {
        Task<IEnumerable<AccountType>> GetAccountTypesAsync();

        Task<IEnumerable<AssetType>> GetAssetTypesAsync();
    }
}
