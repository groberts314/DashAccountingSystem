namespace DashAccountingSystem.Data.Models
{
    public enum TransactionStatus : short
    {
        Unknown = 0,
        Pending = 1,
        Posted = 2,
        Canceled = 3,
        Closed = 4
    }
}
