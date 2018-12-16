using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Models
{
    public class JournalEntryAccountDetailedViewModel : JournalEntryAccountBaseViewModel
    {
        [Display(Name = "Account Name")]
        public string AccountName { get; set; }

        [Display(Name = "Asset Type")]
        public string AssetType { get; set; }

        public static JournalEntryAccountDetailedViewModel FromModel(JournalEntryAccount model)
        {
            if (model == null)
                return null;

            return new JournalEntryAccountDetailedViewModel()
            {
                AccountId = model.AccountId,
                AccountName = model.Account.DisplayName,
                AssetType = model.AssetType.Name,
                AssetTypeId = model.AssetTypeId,
                Credit = model.AmountType == BalanceType.Credit ? Math.Abs(model.Amount) : 0.0m,
                Debit = model.AmountType == BalanceType.Debit ? model.Amount : 0.0m
            };
        }
    }
}
