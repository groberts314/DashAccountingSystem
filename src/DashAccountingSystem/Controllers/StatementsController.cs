using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DashAccountingSystem.Data.Repositories;
using DashAccountingSystem.Filters;
using DashAccountingSystem.Models;

namespace DashAccountingSystem.Controllers
{
    public class StatementsController : Controller
    {
        private readonly ITenantRepository _tenantRepository = null;

        public StatementsController(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Statements", Name = "statementsIndex")]
        [TenantViewDataFilter]
        public async Task<IActionResult> Index(int tenantId)
        {
            await Task.FromResult(0);
            return View();
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Statements/BalanceSheet", Name = "balanceSheet")]
        [TenantViewDataFilter]
        public async Task<IActionResult> BalanceSheet(int tenantId)
        {
            await Task.FromResult(0);
            return View();
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Statements/ProfitAndLoss", Name = "profitAndLoss")]
        [TenantViewDataFilter]
        public async Task<IActionResult> ProfitAndLoss(int tenantId)
        {
            await Task.FromResult(0);
            return View();
        }
    }
}