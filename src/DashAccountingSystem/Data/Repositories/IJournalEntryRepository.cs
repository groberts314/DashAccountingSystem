using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Data.Repositories
{
    public interface IJournalEntryRepository
    {
        Task<JournalEntry> GetByIdAsync(int journalEntryId);
        Task<IEnumerable<JournalEntry>> GetJournalEntriesForMonthAsync(int tenantId, int year, byte month);
        Task<IEnumerable<JournalEntry>> GetJournalEntriesForPeriodAsync(int accountingPeriodId);
        Task<IEnumerable<JournalEntry>> GetJournalEntriesForQuarterAsync(int tenantId, int year, byte quarter);
        Task<IEnumerable<JournalEntry>> GetJournalEntriesForYearAsync(int tenantId, int year);
        Task<JournalEntry> InsertJournalEntryAsync(JournalEntry entry);
        Task<JournalEntry> PostJournalEntryAsync(int entryId, DateTime postDate, Guid postedByUserId);
    }
}
