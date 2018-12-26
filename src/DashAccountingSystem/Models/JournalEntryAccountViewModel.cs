using System;
using System.ComponentModel.DataAnnotations;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Models
{
    public class JournalEntryAccountViewModel
    {
        [Required]
        public int AccountId { get; set; }

        [Display(Name = "Account Name")]
        public string AccountName { get; set; }

        [Required]
        public int AssetTypeId { get; set; }

        [Display(Name = "Asset Type")]
        public string AssetType { get; set; }

        [Required]
        public decimal Debit { get; set; }

        [Required]
        public decimal Credit { get; set; }

        public decimal Amount
        {
            get
            {
                if (AmountType == AmountType.Credit)
                    return -Credit;
                else
                    return Debit;
            }
        }

        public AmountType AmountType
        {
            get
            {
                if (Credit > 0.0m)
                    return AmountType.Credit;
                else
                    return AmountType.Debit;
            }
        }

        public bool HasAmount { get { return Debit > 0.0m || Credit > 0.0m; } }

        public JournalEntryAccount ToModel()
        {
            return new JournalEntryAccount(AccountId, Amount, AssetTypeId);
        }

        public static JournalEntryAccountViewModel FromModel(JournalEntryAccount model)
        {
            if (model == null)
                return null;

            return new JournalEntryAccountViewModel()
            {
                AccountId = model.AccountId,
                AccountName = model.Account.DisplayName,
                AssetType = model.AssetType.Name,
                AssetTypeId = model.AssetTypeId,
                Credit = model.AmountType == AmountType.Credit ? Math.Abs(model.Amount) : 0.0m,
                Debit = model.AmountType == AmountType.Debit ? model.Amount : 0.0m
            };
        }
    }
}
