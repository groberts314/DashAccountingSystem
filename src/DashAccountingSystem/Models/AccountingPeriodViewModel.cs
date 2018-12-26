using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Models
{
    public class AccountingPeriodViewModel
    {
        public int Id { get; set; }
        public AccountingPeriodType PeriodType { get; set; }
        public int Year { get; set; }
        public byte Month { get; set; }
        public byte Quarter { get; set; }
        public string Name { get; set; }

        public static AccountingPeriodViewModel FromModel(AccountingPeriod accountingPeriod)
        {
            if (accountingPeriod == null)
                return null;

            return new AccountingPeriodViewModel()
            {
                Id = accountingPeriod.Id,
                PeriodType = accountingPeriod.PeriodType,
                Year = accountingPeriod.Year,
                Month = accountingPeriod.Month,
                Quarter = accountingPeriod.Quarter,
                Name = accountingPeriod.Name,
            };
        }
    }
}
