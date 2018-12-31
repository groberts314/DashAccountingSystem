using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Models
{
    public class AccountingPeriodViewModel : AccountingPeriodLiteViewModel
    {
        public bool Current { get; set; }
        public bool Selected { get; set; }

        public static new AccountingPeriodViewModel FromModel(AccountingPeriod model)
        {
            if (model == null)
                return null;

            return Fill(new AccountingPeriodViewModel(), model) as AccountingPeriodViewModel;
        }
    }
}
