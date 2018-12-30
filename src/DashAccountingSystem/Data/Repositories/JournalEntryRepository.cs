using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DashAccountingSystem.Data.Models;
using DashAccountingSystem.Extensions;

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
                .Include(je => je.Tenant)
                .SingleOrDefaultAsync();
        }

        public async Task<JournalEntry> GetByTenantAndEntryIdAsync(int tenantId, int entryId)
        {
            return await _db
                .JournalEntry
                .Where(je => je.TenantId == tenantId && je.EntryId == entryId)
                .Include(je => je.Tenant)
                .SingleOrDefaultAsync();
        }

        public async Task<JournalEntry> GetDetailedByIdAsync(int journalEntryId)
        {
            return await _db
                .JournalEntry
                .Where(je => je.Id == journalEntryId)
                .Include(je => je.Tenant)
                .Include(je => je.AccountingPeriod)
                .Include(je => je.CreatedBy)
                .Include(je => je.UpdatedBy)
                .Include(je => je.PostedBy)
                .Include(je => je.CanceledBy)
                .Include(je => je.Accounts)
                    .ThenInclude(jeAcct => jeAcct.Account)
                .Include(je => je.Accounts)
                    .ThenInclude(jeAcct => jeAcct.AssetType)
                .SingleOrDefaultAsync();
        }

        public async Task<JournalEntry> GetDetailedByTenantAndEntryIdAsync(int tenantId, int entryId)
        {
            return await _db
                .JournalEntry
                .Where(je => je.TenantId == tenantId && je.EntryId == entryId)
                .Include(je => je.Tenant)
                .Include(je => je.AccountingPeriod)
                .Include(je => je.CreatedBy)
                .Include(je => je.UpdatedBy)
                .Include(je => je.PostedBy)
                .Include(je => je.CanceledBy)
                .Include(je => je.Accounts)
                    .ThenInclude(jeAcct => jeAcct.Account)
                .Include(je => je.Accounts)
                    .ThenInclude(jeAcct => jeAcct.AssetType)
                .SingleOrDefaultAsync();
        }

        public async Task<PagedResult<JournalEntry>> GetJournalEntriesForPeriodAsync(
            int accountingPeriodId,
            Pagination pagination)
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
                .Include(je => je.Accounts)
                    .ThenInclude(jeAcct => jeAcct.AssetType)
                .GetPagedAsync(pagination);
        }

        public async Task<IEnumerable<JournalEntry>> GetPendingJournalEntriesAsync(int tenantId)
        {
            return await _db
                .JournalEntry
                .Include(je => je.AccountingPeriod)
                .Where(je => je.TenantId == tenantId && je.Status == TransactionStatus.Pending)
                .OrderByDescending(je => je.EntryDate)
                .ThenBy(je => je.Description)
                .Include(je => je.CreatedBy)
                .Include(je => je.PostedBy)
                .Include(je => je.Accounts)
                    .ThenInclude(jeAcct => jeAcct.Account)
                .Include(je => je.Accounts)
                    .ThenInclude(jeAcct => jeAcct.AssetType)
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

            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    var tenant = await _db.Tenant.FirstOrDefaultAsync(t => t.Id == entry.TenantId);
                    if (tenant == null)
                        throw new ArgumentException(
                            $"Journal Entry specifies a non-existent Tenant (ID {entry.TenantId}).",
                            nameof(entry));

                    if (entry.EntryId == 0)
                    {
                        entry.EntryId = await GetNextEntryIdAsync(entry.TenantId);
                    }

                    if (entry.AccountingPeriodId == 0)
                    {
                        var accountingPeriod = await _accountingPeriodRepository
                            .FetchOrCreateAccountingPeriodAsync(
                                entry.TenantId,
                                tenant.AccountingPeriodType,
                                entry.PostDate ?? entry.EntryDate);

                        entry.AccountingPeriodId = accountingPeriod.Id;
                    }

                    await _db.JournalEntry.AddAsync(entry);
                    await _db.SaveChangesAsync();
                    var persistedEntry = await GetDetailedByIdAsync(entry.Id);

                    if (entry.Status == TransactionStatus.Posted)
                        UpdateAccountsForPostedJournalEntry(persistedEntry);
                    else
                        UpdateAccountsForPendingJournalEntry(persistedEntry);

                    await _db.SaveChangesAsync();
                    transaction.Commit();

                    return await GetDetailedByIdAsync(entry.Id);
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task<JournalEntry> PostJournalEntryAsync(int entryId, DateTime postDate, Guid postedByUserId, string note = null)
        {
            var entry = await GetDetailedByIdAsync(entryId);

            if (entry == null)
                return null;

            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    entry.PostDate = postDate;
                    entry.PostedById = postedByUserId;
                    entry.Status = TransactionStatus.Posted;

                    if (!string.IsNullOrWhiteSpace(note) && !string.Equals(note, entry.Note))
                    {
                        entry.Note = note;
                        entry.Updated = DateTime.UtcNow;
                        entry.UpdatedById = postedByUserId;
                    }

                    if (!entry.AccountingPeriod.ContainsDate(postDate))
                    {
                        var postDatePeriod = await _accountingPeriodRepository.FetchOrCreateAccountingPeriodAsync(
                            entry.TenantId,
                            entry.Tenant.AccountingPeriodType,
                            postDate);

                        entry.AccountingPeriodId = postDatePeriod.Id;
                    }

                    UpdateAccountsForPostedJournalEntry(entry);

                    await _db.SaveChangesAsync();
                    transaction.Commit();

                    return await GetDetailedByIdAsync(entryId);
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
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
                if (account.AmountType == AmountType.Debit)
                {
                    account.Account.PendingDebits =
                        (account.Account.PendingDebits ?? 0.0m) + account.Amount;
                }
                else // Credit
                {
                    account.Account.PendingCredits =
                        (account.Account.PendingCredits ?? 0.0m) + Math.Abs(account.Amount);
                }
            }
        }
    }
}
