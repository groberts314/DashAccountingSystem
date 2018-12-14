using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Data.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly ApplicationDbContext _db = null;

        public TenantRepository(ApplicationDbContext applicationDbContext)
        {
            _db = applicationDbContext;
        }

        public async Task<Tenant> GetTenantAsync(int tenantId)
        {
            return await _db
                .Tenant
                .FirstOrDefaultAsync(t => t.Id == tenantId);
        }

        public async Task<IEnumerable<Tenant>> GetTenantsAsync()
        {
            return await _db
                .Tenant
                .OrderBy(t => t.Name)
                .ToListAsync();
        }
    }
}
