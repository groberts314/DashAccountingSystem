using System;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Models
{
    public struct AmountViewModel : IEquatable<AmountViewModel>
    {
        public static readonly AmountViewModel Empty = new AmountViewModel(null, null);

        public decimal? Amount { get; private set; }
        public AssetTypeViewModel AssetType { get; private set; }
        
        public AmountType AmountType
        {
            get
            {
                if (Amount.HasValue && Amount < 0.0m)
                    return AmountType.Credit;
                else
                    return AmountType.Debit;
            }
        }

        public bool HasValue
        {
            get { return Amount.HasValue && AssetType != null; }
        }

        public AmountViewModel(decimal? amount, AssetType assetType)
        {
            Amount = amount;
            AssetType = assetType == null ? null : new AssetTypeViewModel(assetType);
        }

        public AmountViewModel(decimal? amount, int assetTypeId, string assetTypeName)
        {
            Amount = amount;
            AssetType = new AssetTypeViewModel(assetTypeId, assetTypeName);
        }

        public bool Equals(AmountViewModel other)
        {
            if (!HasValue)
                return !other.HasValue;

            return Amount == other.Amount && AssetType.Equals(other.AssetType);
        }

        public override bool Equals(object obj)
        {
            if (obj is AmountViewModel other)
                return Equals(other);

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + (Amount ?? 0.0m).GetHashCode();
                hash = hash * 23 + AssetType?.GetHashCode() ?? 0;
                return hash;
            }
        }
    }
}
