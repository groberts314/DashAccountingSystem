using System;
using System.Collections.Generic;
using System.Linq;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Models
{
    public class JournalEntryDetailedViewModel : JournalEntryBaseViewModel
    {
        public int EntryId { get; set; }

        public TransactionStatus Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }

        public DateTime? CancelDate { get; set; }

        // TODO: Additional properties if needed
        //       Audit User metadata (for CreatedBy / UpdatedBy / EnteredBy / PostedBy / CanceledBy )
        //       Accounting Period Metadata

        public static JournalEntryDetailedViewModel FromModel(JournalEntry model)
        {
            if (model == null)
                return null;

            var viewModel = new JournalEntryDetailedViewModel()
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
                Updated = model.Updated
            };

            viewModel.Accounts = new List<JournalEntryAccountDetailedViewModel>(
                model.Accounts.Select(JournalEntryAccountDetailedViewModel.FromModel));

            return viewModel;
        }
    }
}
