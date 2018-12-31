using System;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Models
{
    public class AccountDisplayViewModel
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int AccountNumber { get; set; }
        public string Name { get; set; }
        public AccountType AccountType { get; set; }
        public AmountType NormalBalanceType { get; set; }
        public DateTime BalanceUpdated { get; set; }
        public bool IsBalanceNormal { get; set; }

        public bool IsPendingBalanceNormal
        {
            get { return PendingBalance.AmountType == NormalBalanceType; }
        }

        public AmountViewModel CurrentBalance { get; set; }
        public AmountViewModel PendingDebits { get; set; }
        public AmountViewModel PendingCredits { get; set; }
        public AmountViewModel PendingBalance { get; set; }

        public static AccountDisplayViewModel FromModel(Account model)
        {
            if (model == null)
                return null;

            var currentbalance = model.CurrentBalance;
            var pendingDebits = model.PendingDebits ?? 0.0m;
            var pendingCredits = model.PendingCredits ?? 0.0m;
            var pendingBalance = currentbalance + pendingDebits - pendingCredits;

            return new AccountDisplayViewModel()
            {
                Id = model.Id,
                TenantId = model.TenantId,
                AccountNumber = model.AccountNumber,
                AccountType = model.AccountType,
                Name = model.Name,
                BalanceUpdated = DateTime.SpecifyKind(model.BalanceUpdated, DateTimeKind.Utc),
                NormalBalanceType = model.NormalBalanceType,
                IsBalanceNormal = model.IsBalanceNormal,
                CurrentBalance = new AmountViewModel(currentbalance, model.AssetType),
                PendingDebits = new AmountViewModel(pendingDebits, model.AssetType),
                PendingCredits = new AmountViewModel(pendingCredits, model.AssetType),
                PendingBalance = new AmountViewModel(pendingBalance, model.AssetType)
            };
        }
    }
}
