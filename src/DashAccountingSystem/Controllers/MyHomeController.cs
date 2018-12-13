using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DashAccountingSystem.Data.Repositories;

namespace DashAccountingSystem.Controllers
{
    public class MyHomeController : Controller
    {
        private readonly ITenantRepository _tenantRepository = null;

        public MyHomeController(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        public async Task<IActionResult> Index()
        {
            // TODO: Get a List of _AUTHORIZED_ Tenants for Current User
            //       If None, render empty view
            //       If 1, redirect to /Ledger/{TenantId}/Accounts
            //       If Multiple, render list view

            var tenants = await _tenantRepository.GetTenantsAsync();

            // If only 1, redirect to /Ledger/{TenantId}/Accounts
            if (tenants.Count() == 1)
            {
                return RedirectToAction("Index", "LedgerAccounts");
            }

            return View(tenants);
        }
    }
}