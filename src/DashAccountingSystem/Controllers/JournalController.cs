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
    public class JournalController : Controller
    {
        private readonly IAccountRepository _accountRepository = null;
        private readonly IAccountingPeriodRepository _accountingPeriodRepository = null;
        private readonly IJournalEntryRepository _journalEntryRepository = null;
        private readonly ISharedLookupRepository _sharedLookupRepository = null;
        private readonly ITenantRepository _tenantRepository = null; 

        public JournalController(
            IAccountRepository accountRepository,
            IAccountingPeriodRepository accountingPeriodRepository,
            IJournalEntryRepository journalEntryRepository,
            ISharedLookupRepository sharedLookupRepository,
            ITenantRepository tenantRepository)
        {
            _accountRepository = accountRepository;
            _accountingPeriodRepository = accountingPeriodRepository;
            _journalEntryRepository = journalEntryRepository;
            _sharedLookupRepository = sharedLookupRepository;
            _tenantRepository = tenantRepository;
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Journal", Name = "journalIndex")]
        public async Task<IActionResult> Index(
            [FromRoute] int tenantId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            // TODO: Either in here or in an attribute, verify authorization for the tenant

            var tenant = await _tenantRepository.GetTenantAsync(tenantId);
            ViewBag.Tenant = tenant;

            // Resolve Current Accounting Period and add to View Bag
            await HydrateViewBagAccountingPeriod(tenant);

            // TODO: Fetch other Past/Closed (or occasionally Future) Periods for a Period Picker ... ???

            var pendingJournalEntries = await _journalEntryRepository.GetPendingJournalEntriesAsync(tenantId);
            var pendingEntriesViewModel = pendingJournalEntries
                .Select(JournalEntryDetailedViewModel.FromModel)
                .ToList();

            // TODO: Select paginated non-Pending journal entries from the period and include in view model

            return View(pendingEntriesViewModel);
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Journal/Period/{accountingPeriodId:int}", Name = "journalPeriodIndex")]
        public async Task<IActionResult> Index(
            [FromRoute] int tenantId,
            [FromRoute] int accountingPeriodId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var tenant = await _tenantRepository.GetTenantAsync(tenantId);
            ViewBag.Tenant = tenant;

            await HydrateViewBagAccountingPeriod(tenant, accountingPeriodId);

            return View();
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Journal/Entry/Add", Name = "addJournalEntry")]
        public async Task<IActionResult> AddEntry(int tenantId)
        {
            // TODO: Either in here or in an attribute, verify authorization for the tenant

            await HydrateViewBagLookupValues(tenantId, true);

            ViewBag.PostBack = false;
            ViewBag.SuccessfulSave = false;

            var timeZoneId = "America/Los_Angeles"; // TODO: Make this a user preference or something...
            var localToday = DateTime.UtcNow.WithTimeZone(timeZoneId).Date;
            var viewModel = new JournalEntryBaseViewModel() { EntryDate = localToday };

            return View(viewModel);
        }

        [HttpPost]
        [Route("Ledger/{tenantId:int}/Journal/Entry/Add", Name = "addJournalEntryPost")]
        public async Task<IActionResult> AddEntry(
            [FromRoute] int tenantId,
            JournalEntryBaseViewModel journalEntryViewModel)
        {
            // TODO: Either in here or in an attribute, verify authorization for the tenant

            ViewBag.PostBack = true;
            ViewBag.SuccessfulSave = false;

            var isJournalEntryValid = ModelState.IsValid;

            if (isJournalEntryValid)
            {
                // Basic view model validation has passed
                // Perform additional validation (i.e. represents a non-empty, valid, balanced transaction)
                // TODO: Move most of this to an attribute hopefully
                isJournalEntryValid = journalEntryViewModel.Validate(ModelState);

                // TODO: Validate Accounting Period based on Post Date coalesce Entry Date
                //       It is an error to try and add a Journal Entry to a Closed Accounting Period!
            }

            if (!isJournalEntryValid)
            {
                await HydrateViewBagLookupValues(tenantId, true);
                return View(journalEntryViewModel);
            }

            var contextUserId = User.GetUserId();

            var journalEntry = new JournalEntry(
                tenantId,
                journalEntryViewModel.EntryDate,
                journalEntryViewModel.PostDate,
                journalEntryViewModel.Description,
                journalEntryViewModel.CheckNumber,
                contextUserId,
                journalEntryViewModel.PostDate.HasValue ? contextUserId : (Guid?)null);

            if (!string.IsNullOrWhiteSpace(journalEntryViewModel.Note))
                journalEntry.Note = journalEntryViewModel.Note;

            journalEntry.Accounts = journalEntryViewModel
                .Accounts
                .Select(a => a.ToModel())
                .ToList();

            var savedJournalEntry = await _journalEntryRepository.CreateJournalEntryAsync(journalEntry);

            var savedJournalEntryViewModel = JournalEntryDetailedViewModel.FromModel(savedJournalEntry);

            ViewBag.Tenant = savedJournalEntry.Tenant;
            ViewBag.SuccessfulSave = true;

            return View(savedJournalEntryViewModel);
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Journal/Entry/{entryId:int}", Name = "journalEntryDetails")]
        public async Task<IActionResult> EntryDetails(int tenantId, int entryId)
        {
            // TODO: Either in here or in an attribute, verify authorization for the tenant
            return await HandleJournalEntryDetailsReadRequest(tenantId, entryId);
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Journal/Entry/{entryId:int}/Edit", Name = "editJournalEntry")]
        public async Task<IActionResult> EditEntry(int tenantId, int entryId)
        {
            var journalEntry = await _journalEntryRepository.GetDetailedByTenantAndEntryIdAsync(tenantId, entryId);

            // If null then 404 not found
            if (journalEntry == null)
                return NotFound();

            var viewModel = JournalEntryDetailedViewModel.FromModel(journalEntry);

            ViewBag.Tenant = journalEntry.Tenant;
            await HydrateViewBagLookupValues(tenantId, false);

            return View(viewModel);
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Journal/Entry/{entryId:int}/Post", Name = "postJournalEntry")]
        public async Task<IActionResult> PostEntry(int tenantId, int entryId)
        {
            // TODO: Either in here or in an attribute, verify authorization for the tenant
            // TODO: Somehow handle invalid request to Post an Entry in any state other than Pending

            ViewBag.PostBack = false;
            ViewBag.SuccessfulSave = false;

            return await HandleJournalEntryDetailsReadRequest(tenantId, entryId);
        }

        [HttpPost]
        [Route("Ledger/{tenantId:int}/Journal/Entry/{entryId:int}/Post", Name = "postJournalEntryPost")]
        public async Task<IActionResult> PostEntry(
            int tenantId,
            int entryId,
            JournalEntryBaseViewModel journalEntryViewModel)
        {
            ViewBag.PostBack = true;
            ViewBag.SuccessfulSave = false;

            var isJournalEntryValid = ModelState.IsValid;

            if (isJournalEntryValid)
            {
                // Validate that it has a Post Date ... not normally required for this view model, but required for Posting!
                if (!journalEntryViewModel.PostDate.HasValue)
                {
                    ModelState.AddModelError("PostDate", "Post date is required to post the journal entry");
                }

                // TODO: Validate Accounting Period based on Post Date
                //       It is an error to try and add a Journal Entry to a Closed Accounting Period!
            }

            if (!isJournalEntryValid)
            {
                return await HandleJournalEntryDetailsReadRequest(tenantId, entryId);
            }

            var contextUserId = User.GetUserId();
            var postedEntry = await _journalEntryRepository.PostJournalEntryAsync(
                journalEntryViewModel.Id,
                journalEntryViewModel.PostDate.Value,
                contextUserId,
                journalEntryViewModel.Note);

            var postedJournalEntryViewModel = JournalEntryDetailedViewModel.FromModel(postedEntry);

            ViewBag.Tenant = postedEntry.Tenant;
            ViewBag.SuccessfulSave = true;

            return View(postedJournalEntryViewModel);
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Journal/Entry/{entryId:int}/Cancel", Name = "cancelPendingJournalEntry")]
        public async Task<IActionResult> CancelPendingEntry(int tenantId, int entryId)
        {
            // TODO: Either in here or in an attribute, verify authorization for the tenant
            return await HandleJournalEntryDetailsReadRequest(tenantId, entryId);
        }

        private async Task<IActionResult> HandleJournalEntryDetailsReadRequest(int tenantId, int entryId)
        {
            var journalEntry = await _journalEntryRepository.GetDetailedByTenantAndEntryIdAsync(tenantId, entryId);

            // If null then 404 not found
            if (journalEntry == null)
                return NotFound();

            var viewModel = JournalEntryDetailedViewModel.FromModel(journalEntry);

            ViewBag.Tenant = journalEntry.Tenant;

            return View(viewModel);
        }

        private async Task HydrateViewBagLookupValues(int tenantId, bool hydrateTenant)
        {
            var accounts = await _accountRepository.GetAccountsByTenantAsync(tenantId);
            var assetTypes = await _sharedLookupRepository.GetAssetTypesAsync();

            ViewBag.AccountList = new CategorizedAccountsViewModel(accounts);
            ViewBag.AssetTypes = assetTypes;

            if (hydrateTenant)
            {
                var tenant = accounts.HasAny() ? accounts.Select(a => a.Tenant).First() : null;
                await HydrateViewBagTenant(tenantId, tenant);
            }
        }

        private async Task HydrateViewBagTenant(int tenantId)
        {
            var fetchedTenant = await _tenantRepository.GetTenantAsync(tenantId);
            ViewBag.Tenant = fetchedTenant;
        }

        private async Task HydrateViewBagTenant(int tenantId, Tenant tenant)
        {
            if (tenant != null)
            {
                ViewBag.Tenant = tenant;
                return;
            }

            await HydrateViewBagTenant(tenantId);
        }

        private async Task HydrateViewBagAccountingPeriod(Tenant tenant, int? accountingPeriodId = null)
        {
            AccountingPeriod accountingPeriod = null;

            if (accountingPeriodId.HasValue)
            {
                accountingPeriod = await _accountingPeriodRepository.GetByIdAsync(accountingPeriodId.Value);
            }
            else
            {
                accountingPeriod = await _accountingPeriodRepository
                    .FetchOrCreateAccountingPeriodAsync(
                        tenant.Id,
                        tenant.AccountingPeriodType,
                        DateTime.Today); // TODO: Time Zone ...???
            }

            ViewBag.AccountingPeriod = accountingPeriod;
        }
    }
}