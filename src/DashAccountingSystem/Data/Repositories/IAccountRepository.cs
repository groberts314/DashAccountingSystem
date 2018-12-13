﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Data.Repositories
{
    public interface IAccountRepository
    {
        Task<Account> CreateAccountAsync(Account account);
        Task<IEnumerable<Account>> GetAccountsByTenantAsync(int tenantId);
        Task<Account> GetAccountByIdAsync(int accountId);
        Task<IEnumerable<JournalEntryAccount>> GetTransactionsByDateRangeAsync(int accountId, DateTime dateRangeStart, DateTime dateRangeEnd);
        Task<IEnumerable<JournalEntryAccount>> GetTransactionsByMonthAsync(int accountId, int year, byte month);
        Task<IEnumerable<JournalEntryAccount>> GetTransactionsByPeriodAsync(int accountId, int accountingPeriodId);
        Task<IEnumerable<JournalEntryAccount>> GetTransactionsByQuarterAsync(int accountId, int year, byte quarter);
        Task<IEnumerable<JournalEntryAccount>> GetTransactionsAsync(int accountId, int pageNumber, int pageSize);
    }
}