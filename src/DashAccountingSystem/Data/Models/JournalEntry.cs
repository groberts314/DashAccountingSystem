using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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
        public int EntryId { get; set; }

        [Required]
        public DateTime EntryDate { get; set; }

        public DateTime? PostDate { get; set; }

        public DateTime? CancelDate { get; set; }

        [Required]
        public TransactionStatus Status { get; set; }

        [Required]
        public int AccountingPeriodId { get; set; }
        public AccountingPeriod AccountingPeriod { get; private set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(2048)]
        public string Description { get; set; }

        public uint? CheckNumber { get; set; }

        public string Note { get; set; }

        public ICollection<JournalEntryAccount> Accounts { get; set; } = new List<JournalEntryAccount>();

        public bool IsBalanced
        {
            get
            {
                if (!Accounts.Any())
                    return true;

                return Accounts.Select(acct => acct.Amount).Sum() == 0.0m;
            }
        }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Created { get; private set; }

        [Required]
        public Guid CreatedById { get; private set; }
        public ApplicationUser CreatedBy { get; private set; }

        public DateTime? Updated { get; set; }

        public Guid? UpdatedById { get; private set; }
        public ApplicationUser UpdatedBy { get; private set; }

        public Guid? PostedById { get; set; }
        public ApplicationUser PostedBy { get; private set; }

        public Guid? CanceledById { get; set; }
        public ApplicationUser CanceledBy { get; private set; }

        /// <summary>
        /// Construct a Journal Entry, explicitly specifying Entry ID and Accounting Period ID
        /// </summary>
        public JournalEntry(
            int tenantId,
            int entryId,
            int accountingPeriodId,
            DateTime entryDate,
            DateTime? postDate,
            string description,
            uint? checkNumber,
            Guid createdById,
            Guid? postedById)
            : this(tenantId, entryDate, postDate, description, checkNumber, createdById, postedById)
        {
            EntryId = entryId;
            AccountingPeriodId = accountingPeriodId;
        }

        /// <summary>
        /// Construct a Journal Entry, without explicitly specifying Entry ID and Accounting Period ID
        /// </summary>
        /// <remarks>
        /// The <see cref="Repositories.JournalEntryRepository"/> will auto-assign the next sequential
        /// Entry ID for the Joural Entry's specified Tenant, and will assign the appropriate
        /// Accounting Period as well when Creating (persisting) a new Journal Entry.
        /// Accounting Period always refers to Post Date if specified (i.e. the Journal Entry is posted)
        /// or otherwise Entry Date (i.e. if the Journal Entry is still pending).
        /// </remarks>
        public JournalEntry(
            int tenantId,
            DateTime entryDate,
            DateTime? postDate,
            string description,
            uint? checkNumber,
            Guid createdById,
            Guid? postedById)
        {
            TenantId = tenantId;
            EntryDate = entryDate;
            PostDate = postDate;
            Description = description;
            CheckNumber = checkNumber;
            CreatedById = createdById;
            PostedById = postedById;
            Status = postDate.HasValue ? TransactionStatus.Posted : TransactionStatus.Pending;
        }
    }
}
