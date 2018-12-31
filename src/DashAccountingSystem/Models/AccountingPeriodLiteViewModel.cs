using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Models
{
    public class AccountingPeriodLiteViewModel
    {
        public int Id { get; set; }
        public AccountingPeriodType PeriodType { get; set; }
        public int Year { get; set; }
        public byte Month { get; set; }
        public byte Quarter { get; set; }
        public string Name { get; set; }
        public bool Closed { get; set; }

        public static AccountingPeriodLiteViewModel FromModel(AccountingPeriod model)
        {
            if (model == null)
                return null;

            return Fill(new AccountingPeriodViewModel(), model);
        }

        public static AccountingPeriodLiteViewModel Fill(
            AccountingPeriodLiteViewModel viewModel,
            AccountingPeriod model)
        {
            viewModel.Id = model.Id;
            viewModel.PeriodType = model.PeriodType;
            viewModel.Year = model.Year;
            viewModel.Month = model.Month;
            viewModel.Quarter = model.Quarter;
            viewModel.Name = model.Name;
            viewModel.Closed = model.Closed;

            return viewModel;
        }
    }
}
