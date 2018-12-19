﻿using System;
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
            // TODO: Either in here or in an attribute, verify authorization for the tenant

            var tenant = await _tenantRepository.GetTenantAsync(tenantId);
            ViewBag.Tenant = tenant;

            var pendingJournalEntries = await _journalEntryRepository.GetPendingJournalEntriesAsync(tenantId);
            var pendingEntriesViewModel = pendingJournalEntries
                .Select(JournalEntryDetailedViewModel.FromModel)
                .ToList();

            return View(pendingEntriesViewModel);
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
            return await HandleJournalEntryDetailsReadRequest(tenantId, entryId);
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
                await HydrateViewTagTenant(tenantId, tenant);
            }
        }

        private async Task HydrateViewTagTenant(int tenantId, Tenant tenant)
        {
            if (tenant != null)
            {
                ViewBag.Tenant = tenant;
                return;
            }

            var fetchedTenant = await _tenantRepository.GetTenantAsync(tenantId);
            ViewBag.Tenant = fetchedTenant;
        }
    }
}