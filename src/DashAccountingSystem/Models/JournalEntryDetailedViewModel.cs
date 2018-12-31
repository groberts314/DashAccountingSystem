using System;
using System.Linq;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Models
{
    public class JournalEntryDetailedViewModel : JournalEntryBaseViewModel, INumberedJournalEntry
    {
        public int EntryId { get; set; }

        public TransactionStatus Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }

        public DateTime? CancelDate { get; set; }

        public AccountingPeriodViewModel Period { get; set; }

        // TODO: Additional properties if needed
        //       Audit User metadata (for CreatedBy / UpdatedBy / EnteredBy / PostedBy / CanceledBy )

        public static JournalEntryDetailedViewModel FromModel(JournalEntry model)
        {
            if (model == null)
                return null;

            return new JournalEntryDetailedViewModel()
            {
                Id = model.Id,
                EntryId = model.EntryId,
                EntryDate = model.EntryDate,
                PostDate = model.PostDate,
                CancelDate = model.CancelDate,
                CheckNumber = model.CheckNumber,
                Description = model.Description,
                Note = model.Note,
                Status = model.Status,
                Created = model.Created,
                Updated = model.Updated,
                Accounts = model.Accounts.Select(JournalEntryAccountViewModel.FromModel),
                Period = AccountingPeriodViewModel.FromModel(model.AccountingPeriod)
            };
        }
    }
}
