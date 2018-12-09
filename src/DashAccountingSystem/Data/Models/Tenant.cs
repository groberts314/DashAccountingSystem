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

        // Navigation Properties
        public ICollection<AccountingPeriod> AccountingPeriods { get; } = new List<AccountingPeriod>();
        public ICollection<Account> Accounts { get; } = new List<Account>();

        public Tenant(string name)
        {
            Name = name;
        }
    }
}
