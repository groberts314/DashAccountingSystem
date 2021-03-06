﻿using System.Collections.Generic;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Models
{
    public class AccountDetailsViewModel
    {
        public AccountDisplayViewModel Account { get; set; }
        public IEnumerable<AccountTransactionViewModel> PendingTransactions { get; set; }
        public PagedResult<AccountTransactionViewModel> PostedTransactions { get; set; }
    }
}
