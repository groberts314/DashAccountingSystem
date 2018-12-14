using DashAccountingSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DashAccountingSystem.Models
{
    public class ChartOfAccountsViewModel : BaseTenantViewModel
    {
        public Dictionary<AccountCategory, Dictionary<AccountSubCategory, IEnumerable<AccountLiteViewModel>>> Accounts { get; private set; }

        public ChartOfAccountsViewModel(Tenant tenant, IEnumerable<Account> accounts, string sectionTitle = "Chart of Accounts")
            : base(tenant, sectionTitle)
        {
            if (accounts == null || !accounts.Any())
                Accounts = InitializeChartOfAccounts(false);

            Accounts = BuildChartOfAccounts(accounts);
        }

        private Dictionary<AccountCategory, Dictionary<AccountSubCategory, IEnumerable<AccountLiteViewModel>>> BuildChartOfAccounts(IEnumerable<Account> accounts)
        {
            var results = InitializeChartOfAccounts(true);
            Dictionary<AccountSubCategory, IEnumerable<AccountLiteViewModel>> categoryEntry = null;

            foreach (var account in accounts)
            {
                var accountSubCategory = (AccountSubCategory)account.AccountTypeId;
                var accountViewModel = AccountLiteViewModel.FromAccount(account);

                switch (accountSubCategory)
                {
                    case AccountSubCategory.Assets:
                    case AccountSubCategory.Liabilities:
                    case AccountSubCategory.OwnersEquity:
                        categoryEntry = results[AccountCategory.BalanceSheet];
                        break;

                    case AccountSubCategory.Revenue:
                    case AccountSubCategory.Expenses:
                        categoryEntry = results[AccountCategory.ProfitAndLoss];
                        break;
                }

                var subCategoryAccountList = categoryEntry[accountSubCategory] as List<AccountLiteViewModel>;
                subCategoryAccountList.Add(accountViewModel);
            }

            return results;
        }

        private Dictionary<AccountCategory, Dictionary<AccountSubCategory, IEnumerable<AccountLiteViewModel>>> InitializeChartOfAccounts(
            bool initializeLists = false)
        {
            var results = Enum
                .GetValues(typeof(AccountCategory))
                .Cast<AccountCategory>()
                .ToDictionary(
                    category => category,
                    category => new Dictionary<AccountSubCategory, IEnumerable<AccountLiteViewModel>>());

            Func<IEnumerable<AccountLiteViewModel>> initSubCategoryList = () => initializeLists
                ? new List<AccountLiteViewModel>()
                : Enumerable.Empty<AccountLiteViewModel>();

            foreach (var category in results.Keys)
            {
                switch (category)
                {
                    case AccountCategory.BalanceSheet:
                        results[category].Add(AccountSubCategory.Assets, initSubCategoryList());
                        results[category].Add(AccountSubCategory.Liabilities, initSubCategoryList());
                        results[category].Add(AccountSubCategory.OwnersEquity, initSubCategoryList());
                        break;
                    case AccountCategory.ProfitAndLoss:
                        results[category].Add(AccountSubCategory.Revenue, initSubCategoryList());
                        results[category].Add(AccountSubCategory.Expenses, initSubCategoryList());
                        break;

                }
            }

            return results;
        }
    }
}
