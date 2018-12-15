using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DashAccountingSystem.Extensions;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Models
{
    public class AddJournalEntryPageViewModel : ChartOfAccountsViewModel
    {
        public IEnumerable<AssetType> AssetTypes { get; set; }

        public JournalEntryBaseViewModel JournalEntry { get; set; }

        public AddJournalEntryPageViewModel(
            Tenant tenant,
            IEnumerable<Account> accounts,
            IEnumerable<AssetType> assetTypes,
            string sectionTitle = "Add New Journal Entry")
            : base (tenant, accounts, sectionTitle)
        {
            AssetTypes = assetTypes;
            JournalEntry = new JournalEntryBaseViewModel();

            var timeZoneId = "America/Los_Angeles"; // TODO: Make this a user preference or something...
            var localToday = DateTime.UtcNow.WithTimeZone(timeZoneId).Date;
            JournalEntry.EntryDate = localToday;
        }
    }
}
