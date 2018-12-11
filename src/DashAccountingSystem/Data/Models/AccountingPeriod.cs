using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DashAccountingSystem.Data.Models
{
    public class AccountingPeriod
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(255)]
        public string Name { get; set; }

        public int TenantId { get; private set; }
        public Tenant Tenant { get; }

        public int Year { get; private set; }

        public byte Month { get; private set; }

        public byte Quarter { get; private set; }

        public AccountingPeriod()
        {

        }

        public AccountingPeriod(int tenantId, int year, byte month)
        {
            if (month < 1 || month > 12)
                throw new ArgumentOutOfRangeException(
                    nameof(month),
                    $"'{nameof(month)}' must be a valid calendar month.  {month} is not a valid value.");

            TenantId = tenantId;
            Year = year;
            Month = month;
            Quarter = (byte)(month / 3 + (month % 3 == 0 ? 0 : 1));

            var periodDate = new DateTime(year, month, 1);
            Name = $"{periodDate.ToString("MMMM")} {year}";

        }

        public bool ContainsDate(DateTime date)
        {
            var periodStart = new DateTime(Year, Month, 1);
            var periodEnd = periodStart.AddMonths(1);
            return date >= periodStart && date < periodEnd;
        }
    }
}
