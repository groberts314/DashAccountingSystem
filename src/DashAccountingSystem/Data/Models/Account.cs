using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DashAccountingSystem.Data.Models
{
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }

        [Required]
        public int TenantId { get; private set; }
        public Tenant Tenant { get; private set; }

        [Required]
        public ushort AccountNumber { get; private set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; private set; }

        public string Description { get; set; }

        [Required]
        public int AccountTypeId { get; private set; }
        public AccountType AccountType { get; private set; }

        [Required]
        public int AssetTypeId { get; private set; }
        public AssetType AssetType { get; private set; }

        [Required]
        public AmountType NormalBalanceType { get; private set; }

        [Required]
        public decimal CurrentBalance { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Created { get; private set; }

        [Required]
        public Guid CreatedById { get; private set; }
        public ApplicationUser CreatedBy { get; private set; }

        public DateTime? Updated { get; set; }

        public Guid? UpdatedById { get; private set; }
        public ApplicationUser UpdatedBy { get; private set; }

        public DateTime BalanceUpdated { get; set; }

        public decimal? PendingDebits { get; set; }

        public decimal? PendingCredits { get; set; }

        public string DisplayName
        {
            get { return $"{AccountNumber} - {Name}"; }
        }

        public bool IsBalanceNormal
        {
            get
            {
                if (CurrentBalance == 0.0m)
                    return true;

                if (NormalBalanceType == AmountType.Debit && CurrentBalance > 0)
                    return true;

                if (NormalBalanceType == AmountType.Credit && CurrentBalance < 0)
                    return true;

                return false;
            }
        }

        public Account(
            int tenantId,
            ushort accountNumber,
            string name,
            string description,
            int accountTypeId,
            int assetTypeId,
            AmountType normalBalanceType,
            Guid createdById)
        {
            TenantId = tenantId;
            AccountNumber = accountNumber;
            Name = name;
            Description = description;
            AccountTypeId = accountTypeId;
            AssetTypeId = assetTypeId;
            NormalBalanceType = normalBalanceType;
            CreatedById = createdById;

            CurrentBalance = 0.0m;
        }
    }
}
