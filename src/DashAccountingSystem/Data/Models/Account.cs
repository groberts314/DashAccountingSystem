using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DashAccountingSystem.Data.Models
{
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }

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
        public BalanceType NormalBalanceType { get; private set; }

        [Required]
        public decimal CurrentBalance { get; private set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Created { get; private set; }

        public DateTime? Updated { get; set; }

        public DateTime BalanceUpdated { get; set; }

        public bool IsBalanceNormal
        {
            get
            {
                if (CurrentBalance == 0.0m)
                    return true;

                if (NormalBalanceType == BalanceType.Debit && CurrentBalance > 0)
                    return true;

                if (NormalBalanceType == BalanceType.Credit && CurrentBalance < 0)
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
            BalanceType normalBalanceType)
        {
            TenantId = tenantId;
            AccountNumber = accountNumber;
            Name = name;
            Description = description;
            AccountTypeId = accountTypeId;
            AssetTypeId = assetTypeId;
            NormalBalanceType = normalBalanceType;
            CurrentBalance = 0.0m;
        }
    }
}
