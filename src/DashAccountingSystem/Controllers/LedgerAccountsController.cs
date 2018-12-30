using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DashAccountingSystem.Data.Models;
using DashAccountingSystem.Data.Repositories;
using DashAccountingSystem.Extensions;
using DashAccountingSystem.Filters;
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
        [TenantViewDataFilter]
        public async Task<IActionResult> Index(int tenantId)
        {
            // TODO: Either in here or in an attribute, verify authorization for the tenant
             
            var accounts = await _accountRepository.GetAccountsByTenantAsync(tenantId);
            var viewModel = new ChartOfAccountsViewModel(accounts);

            return View(viewModel);
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Accounts/Add", Name = "addAccount")]
        [TenantViewDataFilter]
        public async Task<IActionResult> AddAccount(int tenantId)
        {
            // TODO: Either in here or in an attribute, verify authorization for the tenant
            await Task.FromResult(0);
            return View();
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Accounts/{accountId:int}", Name = "accountDetails")]
        [TenantViewDataFilter]
        public async Task<IActionResult> AccountDetails(int tenantId, int accountId)
        {
            // TODO: Either in here or in an attribute, verify authorization for the tenant
            var account = await _accountRepository.GetAccountByIdAsync(accountId);
            if (account == null)
                return NotFound();

            // TODO: If doesn't belong to selected tenant either 404 not found or 403 forbidden

            // TODO: Get pending and recently posted transactions and use an enhanced view model
            //       instead of raw Account model

            return View(account);
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Accounts/{accountId:int}/Edit", Name = "editAccount")]
        [TenantViewDataFilter]
        public async Task<IActionResult> EditAccount(int tenantId, int accountId)
        {
            // TODO: Either in here or in an attribute, verify authorization for the tenant
            var account = await _accountRepository.GetAccountByIdAsync(accountId);
            if (account == null)
                return NotFound();

            return View(account);
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Accounts/{accountId:int}/Close", Name = "closeAccount")]
        [TenantViewDataFilter]
        public async Task<IActionResult> CloseAccount(int tenantId, int accountId)
        {
            // TODO: Either in here or in an attribute, verify authorization for the tenant
            var account = await _accountRepository.GetAccountByIdAsync(accountId);
            if (account == null)
                return NotFound();

            return View(account);
        }
    }
}