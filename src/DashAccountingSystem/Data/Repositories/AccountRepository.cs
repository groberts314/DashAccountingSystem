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
                .Include("AccountType")
                .Include("AssetType")
                .FirstOrDefaultAsync(a => a.Id == accountId);
        }

        public async Task<IEnumerable<Account>> GetAccountsByTenantAsync(int tenantId)
        {
            return await _db
                .Account
                .Include("AccountType")
                .Include("AssetType")
                .Where(a => a.TenantId == tenantId)
                .OrderBy(a => a.AccountTypeId)
                .ThenBy(a => a.AccountNumber)
                .ToListAsync();
        }

        public Task<IEnumerable<JournalEntryAccount>> GetTransactionsAsync(int accountId, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<JournalEntryAccount>> GetTransactionsByDateRangeAsync(int accountId, DateTime dateRangeStart, DateTime dateRangeEnd)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<JournalEntryAccount>> GetTransactionsByMonthAsync(int accountId, int year, byte month)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<JournalEntryAccount>> GetTransactionsByPeriodAsync(int accountId, int accountingPeriodId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<JournalEntryAccount>> GetTransactionsByQuarterAsync(int accountId, int year, byte quarter)
        {
            throw new NotImplementedException();
        }
    }
}
