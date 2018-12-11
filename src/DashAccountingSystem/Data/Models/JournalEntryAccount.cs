using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DashAccountingSystem.Data.Models
{
    public class JournalEntryAccount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }

        [Required]
        public int JournalEntryId { get; private set; }
        public JournalEntry JournalEntry { get; private set; }

        [Required]
        public int AccountId { get; private set; }
        public Account Account { get; private set; }

        [Required]
        public int AssetTypeId { get; private set; }
        public AssetType AssetType { get; private set; }

        [Required]
        public decimal Amount { get; set; }

        public BalanceType AmountType
        {
            get { return Amount >= 0.0m ? BalanceType.Debit : BalanceType.Credit; }
        }

        public decimal? PreviousBalance { get; set; }

        public decimal? NewBalance { get; set; }

        public JournalEntryAccount(int accountId, decimal amount, int assetTypeId)
        {
            AccountId = accountId;
            Amount = amount;
            AssetTypeId = assetTypeId;
        }

        public JournalEntryAccount(int journalEntryId, int accountId, decimal amount, int assetTypeId)
            : this (accountId, amount, assetTypeId)
        {
            JournalEntryId = journalEntryId;
        }
    }
}
