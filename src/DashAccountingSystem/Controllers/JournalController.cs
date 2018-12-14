using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DashAccountingSystem.Data.Repositories;
using DashAccountingSystem.Models;

namespace DashAccountingSystem.Controllers
{
    public class JournalController : Controller
    {
        private readonly IJournalEntryRepository _journalEntryRepository = null;
        private readonly ITenantRepository _tenantRepository = null; 

        public JournalController(
            IJournalEntryRepository journalEntryRepository,
            ITenantRepository tenantRepository)
        {
            _journalEntryRepository = journalEntryRepository;
            _tenantRepository = tenantRepository;
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Journal", Name = "journalIndex")]
        public async Task<IActionResult> Index(int tenantId)
        {
            var tenant = await _tenantRepository.GetTenantAsync(tenantId);
            var viewModel = new JournalIndexViewModel(tenant);

            return View(viewModel);
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Journal/Entry/Add", Name = "addJournalEntry")]
        public IActionResult AddEntry(int tenantId)
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