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

        public bool IsPending
        {
            get { return !PostDate.HasValue; }
        }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(2048)]
        public string Description { get; set; }

        public ushort? CheckNumber { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Created { get; private set; }

        [Required]
        public string EnteredById { get; private set; }
        public IdentityUser EnteredBy { get; private set; }

        public string PostedById { get; private set; }
        public IdentityUser PostedBy { get; private set; }

        public JournalEntry(
            int tenantId,
            int entryId,
            DateTime entryDate,
            DateTime? postDate,
            string description,
            ushort? checkNumber,
            string enteredById,
            string postedById)
        {
            TenantId = tenantId;
            EntryId = entryId;
            EntryDate = entryDate;
            PostDate = postDate;
            Description = description;
            CheckNumber = checkNumber;
            EnteredById = enteredById;
            PostedById = postedById;
        }
    }
}
