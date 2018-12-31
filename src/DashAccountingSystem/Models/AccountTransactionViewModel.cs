using System;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Models
{
    public class AccountTransactionViewModel : INumberedJournalEntry
    {
        // Journal Entry top-level
        public int Id { get; set; }
        public int EntryId { get; set; }
        public TransactionStatus Status { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public uint? CheckNumber { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime? PostDate { get; set; }
        public DateTime? CancelDate { get; set; }

        public DateTime Date
        {
            get
            {
                switch (Status)
                {
                    case TransactionStatus.Posted:
                        return PostDate.Value;
                    case TransactionStatus.Pending:
                    default:
                        return EntryDate;
                }
            }
        }

        public AccountingPeriodLiteViewModel Period { get; set; }

        // Account Level
        public string AssetType { get; set; }
        public decimal Amount { get; set; }

        public AmountType AmountType
        {
            get
            {
                if (Amount < 0.0m)
                    return AmountType.Credit;
                else
                    return AmountType.Debit;
            }
        }

        public decimal? Debit
        {
            get
            {
                if (AmountType == AmountType.Debit)
                    return Amount;

                return null;
            }
        }

        public decimal? Credit
        {
            get
            {
                if (AmountType == AmountType.Credit)
                    return -Amount;

                return null;
            }
        }

        public decimal? PreviousBalance { get; set; }
        public decimal? NewBalance { get; set; }

        public static AccountTransactionViewModel FromModel(JournalEntryAccount model)
        {
            if (model == null)
                return null;

            return new AccountTransactionViewModel()
            {
                Id = model.JournalEntry.Id,
                EntryId = model.JournalEntry.EntryId,
                EntryDate = model.JournalEntry.EntryDate,
                Description = model.JournalEntry.Description,
                Note = model.JournalEntry.Note,
                CheckNumber = model.JournalEntry.CheckNumber,
                Created = model.JournalEntry.Created,
                Updated = model.JournalEntry.Updated,
                PostDate = model.JournalEntry.PostDate,
                CancelDate = model.JournalEntry.CancelDate,
                Status = model.JournalEntry.Status,
                Period = AccountingPeriodLiteViewModel.FromModel(model.JournalEntry.AccountingPeriod),
                AssetType = model.AssetType.Name,
                Amount = model.Amount,
                PreviousBalance = model.PreviousBalance,
                NewBalance = model.NewBalance
            };
        }
    }
}
