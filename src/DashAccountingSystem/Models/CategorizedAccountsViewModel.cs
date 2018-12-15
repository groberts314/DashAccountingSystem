using System;
using System.Collections.Generic;
using System.Linq;
using DashAccountingSystem.Data.Models;
using DashAccountingSystem.Extensions;

namespace DashAccountingSystem.Models
{
    public class CategorizedAccountsViewModel
    {
        public Dictionary<AccountSubCategory, IEnumerable<AccountLiteViewModel>> AccountCategories { get; private set; }

        public CategorizedAccountsViewModel(IEnumerable<Account> accounts)
        {
            if (accounts.IsEmpty())
            {
                AccountCategories = new Dictionary<AccountSubCategory, IEnumerable<AccountLiteViewModel>>();
            }
            else
            {
                AccountCategories = Enum
                    .GetValues(typeof(AccountSubCategory))
                    .Cast<AccountSubCategory>()
                    .ToDictionary(
                        category => category,
                        category => new List<AccountLiteViewModel>() as IEnumerable<AccountLiteViewModel>);

                foreach (var account in accounts)
                {
                    var accountSubCategory = (AccountSubCategory)account.AccountTypeId;
                    var accountViewModel = AccountLiteViewModel.FromAccount(account);
                    var subCategoryAccountList = AccountCategories[accountSubCategory] as List<AccountLiteViewModel>;
                    subCategoryAccountList.Add(accountViewModel);
                }
            }
        }
    }
}
