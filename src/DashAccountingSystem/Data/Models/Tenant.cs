using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DashAccountingSystem.Data.Models
{
    public class Tenant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        // Navigation Properties
        public ICollection<AccountingPeriod> AccountingPeriods { get; } = new List<AccountingPeriod>();

        public Tenant(string name)
        {
            Name = name;
        }
    }
}
