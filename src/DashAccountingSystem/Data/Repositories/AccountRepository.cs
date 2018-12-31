using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Data.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _db = null;

        public AccountRepository(ApplicationDbContext applicationDbContext)
        {
            _db = applicationDbContext;
        }

        public async Task<Account> CreateAccountAsync(Account account)
        {
            account.BalanceUpdated = DateTime.UtcNow;
            await _db.Account.AddAsync(account);
            await _db.SaveChangesAsync();
            return await GetAccountByIdAsync(account.Id);
        }

        public async Task<Account> GetAccountByIdAsync(int accountId)
        {
            return await _db
                .Account
                .Include(a => a.AccountType)
                .Include(a => a.AssetType)
                .Include(a => a.Tenant)
                .Include(a => a.CreatedBy)
                .Include(a => a.UpdatedBy)
                .FirstOrDefaultAsync(a => a.Id == accountId);
        }

        public async Task<IEnumerable<Account>> GetAccountsByTenantAsync(int tenantId)
        {
            return await _db
                .Account
                .Where(a => a.TenantId == tenantId)
                .Include(a => a.AccountType)
                .Include(a => a.AssetType)
                .Include(a => a.Tenant)
                .OrderBy(a => a.AccountTypeId)
                .ThenBy(a => a.AccountNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<JournalEntryAccount>> GetPendingTransactionsAsync(int accountId)
        {
            return await _db
                .JournalEntryAccount
                .Include(jeAcct => jeAcct.JournalEntry)
                .Where(jeAcct =>
                    jeAcct.AccountId == accountId &&
                    jeAcct.JournalEntry.Status == TransactionStatus.Pending)
                .OrderByDescending(jeAcct => jeAcct.JournalEntry.EntryDate)
                .Include(jeAcct => jeAcct.JournalEntry.AccountingPeriod)
                .Include(jeAcct => jeAcct.JournalEntry.CreatedBy)
                .ToListAsync();

        }

        public Task<PagedResult<JournalEntryAccount>> GetPostedTransactionsAsync(int accountId, Pagination pagination)
        {
            throw new NotImplementedException();
        }
    }
}
