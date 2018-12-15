using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DashAccountingSystem.Data.Repositories;
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
        public async Task<IActionResult> Index(int tenantId)
        {
            var tenant = await _tenantRepository.GetTenantAsync(tenantId);
            ViewBag.Tenant = tenant;

            return View();
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Statements/BalanceSheet", Name = "balanceSheet")]
        public IActionResult BalanceSheet(int tenantId)
        {
            return View();
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Statements/ProfitAndLoss", Name = "profitAndLoss")]
        public IActionResult ProfitAndLoss(int tenantId)
        {
            return View();
        }
    }
}