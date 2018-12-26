using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DashAccountingSystem.Data.Models
{
    public class AccountingPeriod
    {
        public static readonly AccountingPeriodType DefaultPeriodType = AccountingPeriodType.Quarter;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(255)]
        public string Name { get; set; }

        public int TenantId { get; private set; }
        public Tenant Tenant { get; }

        public AccountingPeriodType PeriodType { get; private set; }

        public int Year { get; private set; }

        public byte Month { get; private set; }

        public byte Quarter { get; private set; }

        public AccountingPeriod()
        {

        }

        public AccountingPeriod(int tenantId, AccountingPeriodType periodType, int year, byte month)
        {
            if (month < 1 || month > 12)
                throw new ArgumentOutOfRangeException(
                    nameof(month),
                    $"'{nameof(month)}' must be a valid calendar month.  {month} is not a valid value.");

            if (periodType == AccountingPeriodType.Year && month != 12)
                throw new ArgumentException(
                    $"'{nameof(month)}' must be set to 12 to create a period of type '{nameof(AccountingPeriodType.Year)}'",
                    nameof(month));

            if (periodType == AccountingPeriodType.Quarter && month % 3 != 0)
                throw new ArgumentException(
                    $"'{nameof(month)}' must be set to 3, 6, 9 or 12 to create a period of type '{nameof(AccountingPeriodType.Quarter)}'",
                    nameof(month));

            PeriodType = periodType;
            TenantId = tenantId;
            Year = year;
            Month = month;
            Quarter = GetQuarter(month);

            var periodDate = new DateTime(year, month, 1);

            switch (periodType)
            {
                case AccountingPeriodType.Month:
                    Name = $"{periodDate.ToString("MMMM")} {year}";
                    break;
                case AccountingPeriodType.Quarter:
                    Name = $"{year} Q{Quarter}";
                    break;
                case AccountingPeriodType.Year:
                    Name = year.ToString();
                    break;
            }
        }

        public bool ContainsDate(DateTime date)
        {
            DateTime periodStart = DateTime.MinValue, periodEnd = DateTime.MinValue;

            switch (PeriodType)
            {
                case AccountingPeriodType.Month:
                    periodStart = new DateTime(Year, Month, 1);
                    periodEnd = periodStart.AddMonths(1);
                    break;

                case AccountingPeriodType.Quarter:
                    periodStart = GetStartDate(Quarter, Year);
                    periodEnd = periodStart.AddMonths(3);
                    break;

                case AccountingPeriodType.Year:
                    periodStart = new DateTime(Year, 1, 1);
                    periodEnd = periodStart.AddYears(1);
                    break;
            }

            return date >= periodStart && date < periodEnd;
        }

        internal static byte GetQuarter(byte month)
        {
            if (month < 1 || month > 12)
                throw new ArgumentOutOfRangeException(
                    nameof(month),
                    $"'{nameof(month)}' must be a valid calendar month.  {month} is not a valid value.");

            return (byte)(month / 3 + (month % 3 == 0 ? 0 : 1));
        }

        internal static byte GetQuarterEndMonth(byte quarter)
        {
            if (quarter < 1 || quarter > 4)
                throw new ArgumentOutOfRangeException(
                    nameof(quarter),
                    $"'{nameof(quarter)}' must be a valid calendar year quarter number (1-4).  {quarter} is not a valid value.");

            return (byte)(quarter * 3);
        }

        internal static DateTime GetStartDate(byte quarter, int year)
        {
            if (quarter < 1 || quarter > 4)
                throw new ArgumentOutOfRangeException(
                    nameof(quarter),
                    $"'{nameof(quarter)}' must be a valid calendar year quarter number (1-4).  {quarter} is not a valid value.");

            var startMonth = quarter * 3 - 2;
            return new DateTime(year, startMonth, 1);
        }
    }
}
