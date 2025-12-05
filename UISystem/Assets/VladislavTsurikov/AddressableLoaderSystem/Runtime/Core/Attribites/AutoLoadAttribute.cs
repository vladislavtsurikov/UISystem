using System;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class AutoLoadAttribute : Attribute
    {
        public string Address { get; }
        public object Id { get; }

        public AutoLoadAttribute(string address)
        {
            Address = address;
            Id = null;
        }

        public AutoLoadAttribute(string address, object id)
        {
            Address = address;
            Id = id;
        }
    }
}
