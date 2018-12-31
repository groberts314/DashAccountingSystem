using System;
using DashAccountingSystem.Data.Models;

namespace DashAccountingSystem.Models
{
    public class AssetTypeViewModel : IEquatable<AssetTypeViewModel>
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Symbol { get; private set; }
        public string FullName { get; private set; }

        public AssetTypeViewModel(int id, string name)
        {
            Id = id;
            FullName = name;
            ParseAssetTypeName(name);
        }

        public AssetTypeViewModel(AssetType model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            Id = model.Id;
            FullName = model.Name;
            ParseAssetTypeName(model.Name);
        }

        private void ParseAssetTypeName(string fullName)
        {
            var nameSegments = fullName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (nameSegments.Length == 2)
            {
                Name = nameSegments[0];
                Symbol = nameSegments[1];
            }
            else
            {
                Name = fullName;
            }
        }

        public bool Equals(AssetTypeViewModel other)
        {
            if (other == null)
                return false;

            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (obj is AssetTypeViewModel other)
                return Equals(other);

            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}
