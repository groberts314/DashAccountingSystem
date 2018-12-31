using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Models
{
    public interface INumberedJournalEntry
    {
        int EntryId { get; }
        TransactionStatus Status { get; }
    }
}
