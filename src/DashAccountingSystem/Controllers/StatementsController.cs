using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DashAccountingSystem.Controllers
{
    public class StatementsController : Controller
    {
        [HttpGet]
        [Route("Ledger/{tenantId:int}/Statements", Name = "statementsIndex")]
        public IActionResult Index(int tenantId)
        {
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