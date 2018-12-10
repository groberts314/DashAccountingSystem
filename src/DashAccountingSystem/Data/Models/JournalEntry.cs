using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DashAccountingSystem.Data.Models
{
    public class JournalEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }

        [Required]
        public int TenantId { get; private set; }
        public Tenant Tenant { get; private set; }

        [Required]
        public int EntryId { get; private set; }

        [Required]
        public DateTime EntryDate { get; private set; }

        public DateTime? PostDate { get; set; }

        [Required]
        public int AccountingPeriodId { get; set; }
        public AccountingPeriod AccountingPeriod { get; set; }

        public bool IsPending
        {
            get { return !PostDate.HasValue; }
        }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(2048)]
        public string Description { get; set; }

        public ushort? CheckNumber { get; set; }

        public ICollection<JournalEntryAccount> Accounts { get; private set; } = new List<JournalEntryAccount>();

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Created { get; private set; }

        [Required]
        public string EnteredById { get; private set; }
        public ApplicationUser EnteredBy { get; private set; }

        public DateTime? Updated { get; set; }

        public string UpdatedById { get; private set; }
        public ApplicationUser UpdatedBy { get; private set; }

        public string PostedById { get; private set; }
        public ApplicationUser PostedBy { get; private set; }

        public JournalEntry(
            int tenantId,
            int entryId,
            int accountingPeriodId,
            DateTime entryDate,
            DateTime? postDate,
            string description,
            ushort? checkNumber,
            string enteredById,
            string postedById)
        {
            TenantId = tenantId;
            EntryId = entryId;
            AccountingPeriodId = accountingPeriodId;
            EntryDate = entryDate;
            PostDate = postDate;
            Description = description;
            CheckNumber = checkNumber;
            EnteredById = enteredById;
            PostedById = postedById;
        }
    }
}
