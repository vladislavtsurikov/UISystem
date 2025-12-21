using System;

namespace VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class PrefabAddressAttribute : Attribute
    {
        public string Address { get; }

        public PrefabAddressAttribute(string address)
        {
            Address = address;
        }
    }
}
