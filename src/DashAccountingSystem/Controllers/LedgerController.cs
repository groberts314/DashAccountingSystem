using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DashAccountingSystem.Data.Repositories;

namespace DashAccountingSystem.Controllers
{
    public class LedgerController : Controller
    {
        private readonly IAccountRepository _accountRepository = null;

        public LedgerController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<IActionResult> Accounts(int id)
        {
            var accounts = await _accountRepository.GetAccountsByTenantAsync(id);

            return View(accounts);
        }
    }
}