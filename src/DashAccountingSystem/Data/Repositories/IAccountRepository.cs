using System.Collections.Generic;
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
        Task<PagedResult<JournalEntryAccount>> GetPostedTransactionsAsync(int accountId, Pagination pagination);
    }
}
