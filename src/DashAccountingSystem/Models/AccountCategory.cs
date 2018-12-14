using System.ComponentModel.DataAnnotations;
using DashAccountingSystem.Extensions;

namespace DashAccountingSystem.Models
{
    public enum AccountCategory
    {
        [Display(Name = "Balance Sheet")]
        BalanceSheet = 1,

        [Display(Name = "Profit & Loss")]
        ProfitAndLoss = 2
    }

    public static class AccountCategoryExtensions
    {
        public static string GetDisplayName(this AccountCategory accountCategory)
        {
            return EnumerationExtensions.GetDisplayName(accountCategory);
        }
    }
}
