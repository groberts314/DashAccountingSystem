using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Data.Repositories
{
    public class JournalEntryRepository : IJournalEntryRepository
    {
        private readonly ApplicationDbContext _db = null;
        private readonly IAccountingPeriodRepository _accountingPeriodRepository = null;

        public JournalEntryRepository(
            ApplicationDbContext applicationDbContext,
            IAccountingPeriodRepository accountingPeriodRepository)
        {
            _db = applicationDbContext;
            _accountingPeriodRepository = accountingPeriodRepository;
        }

        public async Task<JournalEntry> GetByIdAsync(int journalEntryId)
        {
            return await _db
                .JournalEntry
                .Where(je => je.Id == journalEntryId)
                .Include(je => je.AccountingPeriod)
                .Include(je => je.CreatedBy)
                .Include(je => je.PostedBy)
                .Include(je => je.Accounts)
                    .ThenInclude(jeAcct => jeAcct.Account)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<JournalEntry>> GetJournalEntriesAsync(int tenantId, int pageNumber, int pageSize)
        {
            return await _db
                .JournalEntry
                .Include(je => je.AccountingPeriod)
                .Where(je =>
                    je.TenantId == tenantId)
                .OrderByDescending(je => je.PostDate ?? je.EntryDate)
                .ThenBy(je => je.Description)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .Include(je => je.CreatedBy)
                .Include(je => je.PostedBy)
                .Include(je => je.Accounts)
                    .ThenInclude(jeAcct => jeAcct.Account)
                .ToListAsync();
        }

        public async Task<IEnumerable<JournalEntry>> GetJournalEntriesForMonthAsync(int tenantId, int year, byte month)
        {
            return await _db
                .JournalEntry
                .Include(je => je.AccountingPeriod)
                .Where(je =>
                    je.TenantId == tenantId &&
                    je.AccountingPeriod.Year == year &&
                    je.AccountingPeriod.Month == month)
                .OrderByDescending(je => je.PostDate ?? je.EntryDate)
                .ThenBy(je => je.Description)
                .Include(je => je.CreatedBy)
                .Include(je => je.PostedBy)
                .Include(je => je.Accounts)
                    .ThenInclude(jeAcct => jeAcct.Account)
                .ToListAsync();
        }

        public async Task<IEnumerable<JournalEntry>> GetJournalEntriesForPeriodAsync(int accountingPeriodId)
        {
            return await _db
                .JournalEntry
                .Where(je => je.AccountingPeriodId == accountingPeriodId)
                .OrderByDescending(je => je.PostDate ?? je.EntryDate)
                .ThenBy(je => je.Description)
                .Include(je => je.AccountingPeriod)
                .Include(je => je.CreatedBy)
                .Include(je => je.PostedBy)
                .Include(je => je.Accounts)
                    .ThenInclude(jeAcct => jeAcct.Account)
                .ToListAsync();
        }

        public async Task<IEnumerable<JournalEntry>> GetJournalEntriesForQuarterAsync(int tenantId, int year, byte quarter)
        {
            return await _db
                .JournalEntry
                .Include(je => je.AccountingPeriod)
                .Where(je =>
                    je.TenantId == tenantId &&
                    je.AccountingPeriod.Year == year &&
                    je.AccountingPeriod.Quarter == quarter)
                .OrderByDescending(je => je.PostDate ?? je.EntryDate)
                .ThenBy(je => je.Description)
                .Include(je => je.CreatedBy)
                .Include(je => je.PostedBy)
                .Include(je => je.Accounts)
                    .ThenInclude(jeAcct => jeAcct.Account)
                .ToListAsync();
        }

        public async Task<IEnumerable<JournalEntry>> GetJournalEntriesForYearAsync(int tenantId, int year)
        {
            return await _db
                .JournalEntry
                .Include(je => je.AccountingPeriod)
                .Where(je => je.TenantId == tenantId && je.AccountingPeriod.Year == year)
                .OrderByDescending(je => je.PostDate ?? je.EntryDate)
                .ThenBy(je => je.Description)
                .Include(je => je.CreatedBy)
                .Include(je => je.PostedBy)
                .Include(je => je.Accounts)
                    .ThenInclude(jeAcct => jeAcct.Account)
                .ToListAsync();
        }

        public async Task<int> GetNextEntryIdAsync(int tenantId)
        {
            var maxCurrentEntryId = await _db
                .JournalEntry
                .Where(je => je.TenantId == tenantId)
                .Select(je => je.EntryId)
                .MaxAsync<int, int?>(entryId => entryId) ?? 0;

            return ++maxCurrentEntryId;
        }

        public async Task<JournalEntry> CreateJournalEntryAsync(JournalEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry), "Journal Entry cannot be null");

            // TODO: Is this check appropriate here?
            //       Is there _ever_ a case when it is okay to have an unbalanced transaction?
            //       Is this the responsibility of a business logic layer?
            if (!entry.IsBalanced)
                throw new ArgumentException(
                    "Journal Entry is not balanced!  It cannot be persisted in this state.",
                    nameof(entry));

            using (var transaction = _db.Database.BeginTransaction())
            {
                if (entry.EntryId == 0)
                {
                    entry.EntryId = await GetNextEntryIdAsync(entry.TenantId);
                }

                if (entry.AccountingPeriodId == 0)
                {
                    var accountingPeriod = await _accountingPeriodRepository
                        .FetchOrCreateAccountPeriodAsync(entry.TenantId, entry.PostDate ?? entry.EntryDate);

                    entry.AccountingPeriodId = accountingPeriod.Id;
                }

                await _db.JournalEntry.AddAsync(entry);
                await _db.SaveChangesAsync();
                var persistedEntry = await GetByIdAsync(entry.Id);

                if (entry.Status == TransactionStatus.Posted)
                    UpdateAccountsForPostedJournalEntry(persistedEntry);
                else
                    UpdateAccountsForPendingJournalEntry(persistedEntry);

                await _db.SaveChangesAsync();
                transaction.Commit();
            }

            return await GetByIdAsync(entry.Id);
        }

        public async Task<JournalEntry> PostJournalEntryAsync(int entryId, DateTime postDate, Guid postedByUserId)
        {
            var entry = await GetByIdAsync(entryId);

            if (entry == null)
                return null;

            entry.PostDate = postDate;
            entry.PostedById = postedByUserId;
            entry.Status = TransactionStatus.Posted;

            if (!entry.AccountingPeriod.ContainsDate(postDate))
            {
                var postDatePeriod = await _accountingPeriodRepository.FetchOrCreateAccountPeriodAsync(
                    entry.TenantId,
                    postDate);

                entry.AccountingPeriodId = postDatePeriod.Id;
            }

            UpdateAccountsForPostedJournalEntry(entry);

            await _db.SaveChangesAsync();

            return await GetByIdAsync(entryId);
        }

        private void UpdateAccountsForPostedJournalEntry(JournalEntry entry)
        {
            foreach (var account in entry.Accounts)
            {
                var previousBalance = account.Account.CurrentBalance;
                var newBalance = previousBalance + account.Amount;
                account.PreviousBalance = previousBalance;
                account.NewBalance = newBalance;
                account.Account.CurrentBalance = newBalance;
                account.Account.BalanceUpdated = DateTime.UtcNow;
            }
        }

        private void UpdateAccountsForPendingJournalEntry(JournalEntry entry)
        {
            foreach (var account in entry.Accounts)
            {
                if (account.AmountType == BalanceType.Debit)
                {
                    account.Account.PendingDebits =
                        account.Account.PendingDebits ?? 0.0m + account.Amount;
                }
                else // Credit
                {
                    account.Account.PendingCredits =
                        account.Account.PendingCredits ?? 0.0m + account.Amount;
                }
            }
        }
    }
}
