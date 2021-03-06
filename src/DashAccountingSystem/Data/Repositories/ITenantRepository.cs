﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Data.Repositories
{
    public interface ITenantRepository
    {
        Task<Tenant> GetTenantAsync(int tenantId);
        Task<IEnumerable<Tenant>> GetTenantsAsync();
    }
}
