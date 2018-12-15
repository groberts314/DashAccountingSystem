using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Models
{
    public class JournalEntryAccountBaseViewModel
    {
        [Required]
        public int AccountId { get; set; }

        [Required]
        public int AssetTypeId { get; set; }

        [Required]
        public decimal Debit { get; set; }

        [Required]
        public decimal Credit { get; set; }

        public decimal Amount
        {
            get
            {
                if (AmountType == BalanceType.Credit)
                    return Credit;
                else
                    return Debit;
            }
        }

        public BalanceType AmountType
        {
            get
            {
                if (Credit > 0.0m)
                    return BalanceType.Credit;
                else
                    return BalanceType.Debit;
            }
        }

        public bool HasAmount { get { return Debit > 0.0m || Credit > 0.0m; } }
    }
}
