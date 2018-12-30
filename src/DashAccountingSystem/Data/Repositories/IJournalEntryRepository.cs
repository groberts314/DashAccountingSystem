using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Data.Repositories
{
    public interface IJournalEntryRepository
    {
        Task<JournalEntry> CreateJournalEntryAsync(JournalEntry entry);

        Task<JournalEntry> GetByIdAsync(int journalEntryId);
        Task<JournalEntry> GetByTenantAndEntryIdAsync(int tenantId, int entryId);
        Task<JournalEntry> GetDetailedByIdAsync(int journalEntryId);
        Task<JournalEntry> GetDetailedByTenantAndEntryIdAsync(int tenantId, int entryId);

        Task<PagedResult<JournalEntry>> GetJournalEntriesForPeriodAsync(int accountingPeriodId, Pagination pagination);

        Task<int> GetNextEntryIdAsync(int tenantId);

        Task<IEnumerable<JournalEntry>> GetPendingJournalEntriesAsync(int tenantId);

        Task<JournalEntry> PostJournalEntryAsync(int entryId, DateTime postDate, Guid postedByUserId, string note = null);
    }
}
