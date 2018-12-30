using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Models
{
    public class JournalViewModel
    {
        public List<JournalEntryDetailedViewModel> PendingEntries { get; set; }
        public PagedResult<JournalEntryDetailedViewModel> Entries { get; set; }
    }
}
