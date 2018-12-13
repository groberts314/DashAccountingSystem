using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DashAccountingSystem.Data.Repositories;

namespace DashAccountingSystem.Controllers
{
    public class JournalController : Controller
    {
        public readonly IJournalEntryRepository _journalEntryRepository = null;

        public JournalController(IJournalEntryRepository journalEntryRepository)
        {
            _journalEntryRepository = journalEntryRepository;
        }

        [HttpGet]
        [Route("Ledger/{tenantId:int}/Journal", Name = "journalIndex")]
        public IActionResult Index(int tenantId)
        {
            return View();
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