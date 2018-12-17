using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using DashAccountingSystem.Extensions;

namespace DashAccountingSystem.Models
{
    public class JournalEntryBaseViewModel
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Entry Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime EntryDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Post Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? PostDate { get; set; }

        [Display(Name ="Description")]
        [MaxLength(2048)]
        [Required(AllowEmptyStrings = false)]
        public string Description { get; set; }

        [Display(Name = "Note")]
        public string Note { get; set; }

        [Display(Name = "Check Number")]
        public uint? CheckNumber { get; set; }

        [Display(Name = "Accounts")]
        public virtual IEnumerable<JournalEntryAccountViewModel> Accounts { get; set; } = new List<JournalEntryAccountViewModel>();

        public bool Validate(ModelStateDictionary modelState)
        {
            if (Accounts.IsEmpty())
                modelState.AddModelError("Accounts", "Journal Entry does not have any accounts");

            var invalidAccounts = Accounts.Where(acct => !acct.HasAmount);

            var accountsGroupedByAssetType = Accounts
                .GroupBy(acct => acct.AssetTypeId)
                .ToDictionary(grp => grp.Key, grp => grp.Select(a => a));

            var deficientAssetTypeGroups = accountsGroupedByAssetType
                .Where(assetTypeGroup => assetTypeGroup.Value.Count() < 2);

            if (deficientAssetTypeGroups.Any())
                foreach (var assetTypeGroup in deficientAssetTypeGroups)
                {
                    // TODO: Resolve name of the offending asset type(s)
                    modelState.AddModelError(
                        "Accounts",
                        $"Journal Entry has fewer than two account entries for asset type ID {assetTypeGroup.Key}");
                }

            var unbalancedAssetTypeGroups = accountsGroupedByAssetType
                .Where(assetTypeGroup =>
                    assetTypeGroup.Value.Sum(a => a.Debit) != assetTypeGroup.Value.Sum(a => a.Credit));

            if (unbalancedAssetTypeGroups.Any())
            {
                foreach (var assetTypeGroup in unbalancedAssetTypeGroups)
                {
                    // TODO: Resolve name of the offending asset type(s)
                    modelState.AddModelError(
                        "Accounts",
                        $"Journal Entry accounts do not balance for asset type ID {assetTypeGroup.Key}");
                }
            }

            return modelState.IsValid;
        }
    }
}
