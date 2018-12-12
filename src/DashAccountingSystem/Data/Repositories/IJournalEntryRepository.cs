﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Data.Repositories
{
    public interface IJournalEntryRepository
    {
        Task<JournalEntry> CreateJournalEntryAsync(JournalEntry entry);

        Task<JournalEntry> GetByIdAsync(int journalEntryId);

        Task<IEnumerable<JournalEntry>> GetJournalEntriesAsync(int tenantId, int pageNumber, int pageSize);
        Task<IEnumerable<JournalEntry>> GetJournalEntriesForMonthAsync(int tenantId, int year, byte month);
        Task<IEnumerable<JournalEntry>> GetJournalEntriesForPeriodAsync(int accountingPeriodId);
        Task<IEnumerable<JournalEntry>> GetJournalEntriesForQuarterAsync(int tenantId, int year, byte quarter);
        Task<IEnumerable<JournalEntry>> GetJournalEntriesForYearAsync(int tenantId, int year);

        Task<int> GetNextEntryIdAsync(int tenantId);

        Task<JournalEntry> PostJournalEntryAsync(int entryId, DateTime postDate, Guid postedByUserId);
    }
}
