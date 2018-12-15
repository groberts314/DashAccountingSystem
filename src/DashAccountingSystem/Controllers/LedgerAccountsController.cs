using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DashAccountingSystem.Data.Models;
using DashAccountingSystem.Data.Repositories;
using DashAccountingSystem.Extensions;
using DashAccountingSystem.Models;

namespace DashAccountingSystem.Controllers
{
    public class LedgerAccountsController : Controller
    {
        private readonly IAccountRepository _accountRepository = null;
        private readonly ITenantRepository _tenantRepository = null;

        public LedgerAccountsController(
            IAccountRepository accountRepository,
            ITenantRepository tenantRepository)
        {
            _accountRepository = accountRepository;
            _tenantRepository = tenantRepository;
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Accounts", Name = "accountsIndex")]
        public async Task<IActionResult> Index(int tenantId)
        {
            // TODO: Either in here or in an attribute, verify authorization for the tenant
             
            var accounts = await _accountRepository.GetAccountsByTenantAsync(tenantId);

            var tenant = accounts.IsEmpty()
                ? await _tenantRepository.GetTenantAsync(tenantId)
                : accounts.Select(a => a.Tenant).First();

            ViewBag.Tenant = tenant;
            var viewModel = new ChartOfAccountsViewModel(accounts);

            return View(viewModel);
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

            // TODO: If null then 404 not found
            // TODO: If doesn't belong to selected tenant either 404 not found or 403 forbidden

            // TODO: Get pending and recently posted transactions and use an enhanced view model
            //       instead of raw Account model

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