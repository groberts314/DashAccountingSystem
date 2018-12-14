using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Models
{
    public class AccountDetailsViewModel : BaseTenantViewModel
    {
        public Account Account { get; private set; }

        public AccountDetailsViewModel(Account account)
            : base(account.Tenant, account.DisplayName)
        {
            Account = account;
        }
    }
}
