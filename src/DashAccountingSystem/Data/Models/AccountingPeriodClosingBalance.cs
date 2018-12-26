using System.ComponentModel.DataAnnotations;

namespace DashAccountingSystem.Data.Models
{
    public class AccountingPeriodClosingBalance
    {
        [Key]
        public int AccountId { get; set; }
        public Account Account { get; private set; }

        [Key]
        public int AccountingPeriodId { get; set; }
        public AccountingPeriod AccountingPeriod { get; private set; }

        public decimal ClosingBalance { get; set; }
    }
}
