using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Models
{
    public class AccountingPeriodViewModel
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public byte Month { get; set; }
        public string MonthName { get; set; }
        public byte Quarter { get; set; }
        public string QuarterName { get; set; }

        public static AccountingPeriodViewModel FromModel(AccountingPeriod accountingPeriod)
        {
            if (accountingPeriod == null)
                return null;

            return new AccountingPeriodViewModel()
            {
                Id = accountingPeriod.Id,
                Year = accountingPeriod.Year,
                Month = accountingPeriod.Month,
                MonthName = accountingPeriod.Name,
                Quarter = accountingPeriod.Quarter,
                QuarterName = accountingPeriod.QuarterName
            };
        }
    }
}
