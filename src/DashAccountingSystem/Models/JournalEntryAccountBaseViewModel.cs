using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DashAccountingSystem.Models
{
    public class JournalEntryAccountBaseViewModel
    {
        [Required]
        public int AccountId { get; set; }

        [Required]
        public int AssetTypeId { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }
}
