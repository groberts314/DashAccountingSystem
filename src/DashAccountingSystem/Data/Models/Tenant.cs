using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DashAccountingSystem.Data.Models
{
    public class Tenant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(255)]
        public string Name { get; set; }

        /// <summary>
        /// The kind of Accounting Period to use for this Tenant's ledger
        /// </summary>
        public AccountingPeriodType AccountingPeriodType { get; set; }

        // Navigation Properties
        public ICollection<AccountingPeriod> AccountingPeriods { get; } = new List<AccountingPeriod>();
        public ICollection<Account> Accounts { get; } = new List<Account>();
        public ICollection<JournalEntry> JournalEntries { get; } = new List<JournalEntry>();

        /// <summary>
        /// Instantiate a new Tenant with system default Accounting Period Type
        /// </summary>
        /// <param name="name"></param>
        public Tenant(string name)
            : this(name, AccountingPeriod.DefaultPeriodType)
        {
        }

        /// <summary>
        /// Instantiate a new Tenant, specifying the Tenant's preferred Accounting Period Type
        /// </summary>
        /// <param name="name"></param>
        /// <param name="accountingPeriodType"></param>
        public Tenant(string name, AccountingPeriodType accountingPeriodType)
        {
            Name = name;
            AccountingPeriodType = accountingPeriodType;
        }
    }
}
