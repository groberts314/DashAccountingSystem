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
    public class JournalController : Controller
    {
        private readonly IAccountRepository _accountRepository = null;
        private readonly IJournalEntryRepository _journalEntryRepository = null;
        private readonly ISharedLookupRepository _sharedLookupRepository = null;

        public JournalController(
            IAccountRepository accountRepository,
            IJournalEntryRepository journalEntryRepository,
            ISharedLookupRepository sharedLookupRepository)
        {
            _accountRepository = accountRepository;
            _journalEntryRepository = journalEntryRepository;
            _sharedLookupRepository = sharedLookupRepository;
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Journal", Name = "journalIndex")]
        [TenantViewDataFilter]
        [PaginationValidationFilter]
        public async Task<IActionResult> Index(
            [FromRoute] int tenantId,
            [FromQuery] PaginationViewModel pagination)
        {
            // TODO: Either in here or in an attribute, verify authorization for the tenant
            return await HandleJournalEntryIndexRequest();
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Journal/Period/{accountingPeriodId:int}", Name = "journalPeriodIndex")]
        [TenantViewDataFilter]
        [PaginationValidationFilter]
        public async Task<IActionResult> Index(
            [FromRoute] int tenantId,
            [FromRoute] int accountingPeriodId,
            [FromQuery] PaginationViewModel pagination)
        {
            // TODO: Either in here or in an attribute, verify authorization for the tenant
            //       and that the specified period belongs to the tenant
            return await HandleJournalEntryIndexRequest();
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Journal/Entry/Add", Name = "addJournalEntry")]
        [TenantViewDataFilter]
        public async Task<IActionResult> AddEntry(int tenantId)
        {
            // TODO: Either in here or in an attribute, verify authorization for the tenant

            await HydrateViewBagLookupValues(tenantId);

            ViewBag.PostBack = false;
            ViewBag.SuccessfulSave = false;

            var timeZoneId = "America/Los_Angeles"; // TODO: Make this a user preference or something...
            var localToday = DateTime.UtcNow.WithTimeZone(timeZoneId).Date;
            var viewModel = new JournalEntryBaseViewModel() { EntryDate = localToday };

            return View(viewModel);
        }

        [HttpPost]
        [Route("Ledger/{tenantId:int}/Journal/Entry/Add", Name = "addJournalEntryPost")]
        [TenantViewDataFilter]
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
                await HydrateViewBagLookupValues(tenantId);
                return View(journalEntryViewModel);
            }

            var contextUserId = User.GetUserId();
            var postingUserId = journalEntryViewModel.PostDate.HasValue ? contextUserId : (Guid?)null;

            var journalEntry = new JournalEntry(
                tenantId,
                journalEntryViewModel.EntryDate,
                journalEntryViewModel.PostDate,
                journalEntryViewModel.Description,
                journalEntryViewModel.CheckNumber,
                contextUserId,
                postingUserId);

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
        [TenantViewDataFilter]
        public async Task<IActionResult> EntryDetails(int tenantId, int entryId)
        {
            // TODO: Either in here or in an attribute, verify authorization for the tenant
            return await HandleJournalEntryDetailsReadRequest(tenantId, entryId);
        }

        [HttpGet]
        [Route(
            "Ledger/{tenantId:int}/Journal/Period/{accountingPeriodId:int}/Entry/{entryId:int}",
            Name = "journalPeriodEntryDetails")]
        [TenantViewDataFilter]
        public async Task<IActionResult> EntryDetails(int tenantId, int accountingPeriodId, int entryId)
        {
            // TODO: Either in here or in an attribute, verify authorization for the tenant
            return await HandleJournalEntryDetailsReadRequest(tenantId, entryId);
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Journal/Entry/{entryId:int}/Edit", Name = "editJournalEntry")]
        [TenantViewDataFilter]
        public async Task<IActionResult> EditEntry(int tenantId, int entryId)
        {
            var journalEntry = await _journalEntryRepository.GetDetailedByTenantAndEntryIdAsync(tenantId, entryId);

            // If null then 404 not found
            if (journalEntry == null)
                return NotFound();

            await HydrateViewBagLookupValues(tenantId);
            var viewModel = JournalEntryDetailedViewModel.FromModel(journalEntry);

            return View(viewModel);
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Journal/Entry/{entryId:int}/Post", Name = "postJournalEntry")]
        [TenantViewDataFilter]
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
        [TenantViewDataFilter]
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
        [TenantViewDataFilter]
        public async Task<IActionResult> CancelPendingEntry(int tenantId, int entryId)
        {
            // TODO: Either in here or in an attribute, verify authorization for the tenant
            return await HandleJournalEntryDetailsReadRequest(tenantId, entryId);
        }

        private async Task<IActionResult> HandleJournalEntryIndexRequest()
        {
            var selectedPeriod = ViewBag.SelectedPeriod as AccountingPeriodViewModel;

            var pendingJournalEntries = await _journalEntryRepository
                .GetPendingJournalEntriesForPeriodAsync(selectedPeriod.Id);

            var pendingEntriesViewModel = pendingJournalEntries
                .Select(JournalEntryDetailedViewModel.FromModel)
                .ToList();

            var pagination = ViewBag.Pagination as Pagination;

            var paginatedEntries = await _journalEntryRepository.GetJournalEntriesForPeriodAsync(
                selectedPeriod.Id,
                pagination);

            var paginatedEntriesViewModel = new PagedResult<JournalEntryDetailedViewModel>()
            {
                Pagination = paginatedEntries.Pagination,
                Results = paginatedEntries
                    .Results
                    .Select(JournalEntryDetailedViewModel.FromModel)
                    .ToList()
            };

            var resultViewModel = new JournalViewModel()
            {
                PendingEntries = null,
                Entries = paginatedEntriesViewModel
            };

            return View(resultViewModel);
        }

        private async Task<IActionResult> HandleJournalEntryDetailsReadRequest(int tenantId, int entryId)
        {
            var journalEntry = await _journalEntryRepository.GetDetailedByTenantAndEntryIdAsync(tenantId, entryId);

            // If null then 404 not found
            if (journalEntry == null)
                return NotFound();

            var viewModel = JournalEntryDetailedViewModel.FromModel(journalEntry);

            return View(viewModel);
        }

        private async Task HydrateViewBagLookupValues(int tenantId)
        {
            var accounts = await _accountRepository.GetAccountsByTenantAsync(tenantId);
            var assetTypes = await _sharedLookupRepository.GetAssetTypesAsync();

            ViewBag.AccountList = new CategorizedAccountsViewModel(accounts);
            ViewBag.AssetTypes = assetTypes;
        }
    }
}