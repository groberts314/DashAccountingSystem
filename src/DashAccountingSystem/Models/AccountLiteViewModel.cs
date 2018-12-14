using System;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Models
{
    public class AccountLiteViewModel
    {
        public int Id { get; set; }
        public int AccountNumber { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public BalanceType NormalBalanceType { get; set; }
        public bool IsBalanceNormal { get; set; }
        public DateTime BalanceUpdated { get; set; }
        public string AssetType { get; set; }
        public decimal CurrentBalance { get; set; }

        public static AccountLiteViewModel FromAccount(Account account)
        {
            if (account == null)
                return null;

            return new AccountLiteViewModel()
            {
                Id = account.Id,
                AccountNumber = account.AccountNumber,
                Name = account.Name,
                DisplayName = account.DisplayName,
                NormalBalanceType = account.NormalBalanceType,
                IsBalanceNormal = account.IsBalanceNormal,
                BalanceUpdated = account.BalanceUpdated,
                AssetType = account.AccountType.Name,
                CurrentBalance = account.CurrentBalance
            };
        }
    }
}
