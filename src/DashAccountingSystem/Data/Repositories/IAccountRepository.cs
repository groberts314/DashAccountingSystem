using System;
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
        Task<IEnumerable<JournalEntryAccount>> GetPendingTransactionsAsync(int accountId);
        Task<IEnumerable<JournalEntryAccount>> GetPostedTransactionsByDateRangeAsync(int accountId, DateTime dateRangeStart, DateTime dateRangeEnd);
        Task<IEnumerable<JournalEntryAccount>> GetPostedTransactionsByMonthAsync(int accountId, int year, byte month);
        Task<IEnumerable<JournalEntryAccount>> GetPostedTransactionsByPeriodAsync(int accountId, int accountingPeriodId);
        Task<IEnumerable<JournalEntryAccount>> GetPostedTransactionsByQuarterAsync(int accountId, int year, byte quarter);
        Task<IEnumerable<JournalEntryAccount>> GetPostedTransactionsAsync(int accountId, int pageNumber, int pageSize);
    }
}
