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
                .Include("JouralEntryAccount")
                .Include("JouralEntryAccount.Account")
                .Include("JournalEntryAccount.AssetType")
                .Include("AccountingPeriod")
                .Include("CreatedBy")
                .Include("PostedBy")
                .FirstOrDefaultAsync(je => je.Id == journalEntryId);
        }

        public async Task<IEnumerable<JournalEntry>> GetJournalEntriesForMonthAsync(int tenantId, int year, byte month)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<JournalEntry>> GetJournalEntriesForPeriodAsync(int accountingPeriodId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<JournalEntry>> GetJournalEntriesForQuarterAsync(int tenantId, int year, byte quarter)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<JournalEntry>> GetJournalEntriesForYearAsync(int tenantId, int year)
        {
            throw new NotImplementedException();
        }

        public async Task<JournalEntry> InsertJournalEntryAsync(JournalEntry entry)
        {
            await _db.JournalEntry.AddAsync(entry);
            await _db.SaveChangesAsync();
            return entry;
        }

        public async Task<JournalEntry> PostJournalEntryAsync(int entryId, DateTime postDate, Guid postedByUserId)
        {
            var entry = await GetByIdAsync(entryId);

            if (entry == null)
                return null;

            entry.PostDate = postDate;
            entry.PostedById = postedByUserId;

            if (!entry.AccountingPeriod.ContainsDate(postDate))
            {
                var postDatePeriod = await _accountingPeriodRepository.FetchOrCreateAccountPeriodAsync(
                    entry.TenantId,
                    postDate.Year,
                    (byte)postDate.Month);

                entry.AccountingPeriodId = postDatePeriod.Id;
            }

            foreach (var account in entry.Accounts)
            {
                var previousBalance = account.Account.CurrentBalance;
                var newBalance = previousBalance + account.Amount;
                account.PreviousBalance = previousBalance;
                account.NewBalance = newBalance;
                account.Account.CurrentBalance = newBalance;
                account.Account.BalanceUpdated = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();

            return await GetByIdAsync(entryId);
        }
    }
}
