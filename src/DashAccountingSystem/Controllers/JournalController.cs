using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DashAccountingSystem.Data.Repositories;
using DashAccountingSystem.Extensions;
using DashAccountingSystem.Models;

namespace DashAccountingSystem.Controllers
{
    public class JournalController : Controller
    {
        private readonly IAccountRepository _accountRepository = null;
        private readonly IJournalEntryRepository _journalEntryRepository = null;
        private readonly ISharedLookupRepository _sharedLookupRepository = null;
        private readonly ITenantRepository _tenantRepository = null; 

        public JournalController(
            IAccountRepository accountRepository,
            IJournalEntryRepository journalEntryRepository,
            ISharedLookupRepository sharedLookupRepository,
            ITenantRepository tenantRepository)
        {
            _accountRepository = accountRepository;
            _journalEntryRepository = journalEntryRepository;
            _sharedLookupRepository = sharedLookupRepository;
            _tenantRepository = tenantRepository;
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Journal", Name = "journalIndex")]
        public async Task<IActionResult> Index(int tenantId)
        {
            var tenant = await _tenantRepository.GetTenantAsync(tenantId);
            ViewBag.Tenant = tenant;

            return View();
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Journal/Entry/Add", Name = "addJournalEntry")]
        public async Task<IActionResult> AddEntry(int tenantId)
        {
            var accounts = await _accountRepository.GetAccountsByTenantAsync(tenantId);

            var tenant = accounts.IsEmpty()
                ? await _tenantRepository.GetTenantAsync(tenantId)
                : accounts.Select(a => a.Tenant).First();

            var assetTypes = await _sharedLookupRepository.GetAssetTypesAsync();

            ViewBag.AccountList = new CategorizedAccountsViewModel(accounts);
            ViewBag.AssetTypes = assetTypes;
            ViewBag.Tenant = tenant;

            var timeZoneId = "America/Los_Angeles"; // TODO: Make this a user preference or something...
            var localToday = DateTime.UtcNow.WithTimeZone(timeZoneId).Date;
            var viewModel = new JournalEntryBaseViewModel() { EntryDate = localToday };

            return View(viewModel);
        }

        [HttpPost]
        [Route("Ledger/{tenantId:int}/Journal/Entry/Add", Name = "addJournalEntryPost")]
        public async Task<IActionResult> AddEntry(
            [FromRoute] int tenantId,
            [FromBody] JournalEntryBaseViewModel journalEntry)
        {


            return View();
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Journal/Entry/{entryId:int}", Name = "journalEntryDetails")]
        public IActionResult EntryDetails(int tenantId, int entryId)
        {
            return View();
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Journal/Entry/{entryId:int}/Edit", Name = "editJournalEntry")]
        public IActionResult EditEntry(int tenantId, int entryId)
        {
            return View();
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Journal/Entry/{entryId:int}/Post", Name = "postJournalEntry")]
        public IActionResult PostEntry(int tenantId, int entryId)
        {
            return View();
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Journal/Entry/{entryId:int}/Delete", Name = "deleteJournalEntry")]
        public IActionResult DeleteEntry(int tenantId, int entryId)
        {
            return View();
        }
    }
}