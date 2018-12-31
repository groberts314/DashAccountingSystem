using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DashAccountingSystem.Data.Models;
using DashAccountingSystem.Data.Repositories;
using DashAccountingSystem.Filters;
using DashAccountingSystem.Models;

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
        [PaginationValidationFilter]
        public async Task<IActionResult> AccountDetails(
            [FromRoute] int tenantId,
            [FromRoute] int accountId,
            [FromQuery] PaginationViewModel pagination)
        {
            // TODO: Either in here or in an attribute, verify authorization for the tenant
            var account = await _accountRepository.GetAccountByIdAsync(accountId);
            if (account == null)
                return NotFound();

            // TODO: If doesn't belong to selected tenant either 404 not found or 403 forbidden

            // Get pending transactions
            var pendingTransactions = await _accountRepository.GetPendingTransactionsAsync(accountId);

            // Get posted transactions
            var paginationModel = ViewBag.Pagination as Pagination;
            var paginatedPostedTransactions = await _accountRepository.GetPostedTransactionsAsync(
                accountId,
                paginationModel);

            var resultViewModel = new AccountDetailsViewModel()
            {
                Account = account,
                PendingTransactions = pendingTransactions.Select(AccountTransactionViewModel.FromModel),
                PostedTransactions = new PagedResult<AccountTransactionViewModel>()
                {
                    Pagination = paginatedPostedTransactions.Pagination,
                    Results = paginatedPostedTransactions.Results.Select(AccountTransactionViewModel.FromModel)
                }
            };

            return View(resultViewModel);
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