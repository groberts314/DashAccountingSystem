using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using DashAccountingSystem.Extensions;

namespace DashAccountingSystem.Models
{
    public class JournalEntryBaseViewModel
    {
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
        [Required(AllowEmptyStrings = false)]
        public string Description { get; set; }

        [Display(Name = "Note")]
        public string Note { get; set; }

        [Display(Name = "Check Number")]
        public uint? CheckNumber { get; set; }

        [Display(Name = "Accounts")]
        public IEnumerable<JournalEntryAccountBaseViewModel> Accounts { get; set; } = new List<JournalEntryAccountBaseViewModel>();

        public bool IsBalanced
        {
            get
            {
                if (!Accounts.HasAny())
                    return true;

                var groupedByAssetType = Accounts
                    .GroupBy(acct => acct.AssetTypeId);

                return groupedByAssetType.All(grp => grp.Select(acct => acct.Amount).Sum() == 0.0m);
            }
        }

        public bool IsValid
        {
            get
            {
                return Accounts != null && Accounts.Count() >= 2 && IsBalanced;
            }
        }
    }
}
