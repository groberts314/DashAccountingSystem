using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DashAccountingSystem.Data.Repositories;

namespace DashAccountingSystem.Controllers
{
    public class LedgerAccountsController : Controller
    {
        private readonly IAccountRepository _accountRepository = null;

        public LedgerAccountsController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Accounts", Name = "accountsIndex")]
        public async Task<IActionResult> Index(int tenantId)
        {
            // TODO: Either in here or in an attribute, verify authorization for the tenant

            var accounts = await _accountRepository.GetAccountsByTenantAsync(tenantId);

            return View(accounts);
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Accounts/Add", Name = "addAccount")]
        public IActionResult AddAccount(int tenantId)
        {
            // TODO: Either in here or in an attribute, verify authorization for the tenant

            return View();
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Accounts/{accountId:int}", Name = "accountDetails")]
        public async Task<IActionResult> AccountDetails(int tenantId, int accountId)
        {
            // TODO: Either in here or in an attribute, verify authorization for the tenant
            var account = await _accountRepository.GetAccountByIdAsync(accountId);

            // TODO: Get pending and recently posted transactions and assemble an appropriate view model

            return View(account);
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Accounts/{accountId:int}/Edit", Name = "editAccount")]
        public async Task<IActionResult> EditAccount(int tenantId, int accountId)
        {
            // TODO: Either in here or in an attribute, verify authorization for the tenant
            var account = await _accountRepository.GetAccountByIdAsync(accountId);

            // TODO: Get pending and recently posted transactions and assemble an appropriate view model

            return View(account);
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Accounts/{accountId:int}/Close", Name = "closeAccount")]
        public async Task<IActionResult> CloseAccount(int tenantId, int accountId)
        {
            // TODO: Either in here or in an attribute, verify authorization for the tenant
            var account = await _accountRepository.GetAccountByIdAsync(accountId);

            // TODO: Get pending and recently posted transactions and assemble an appropriate view model

            return View(account);
        }
    }
}