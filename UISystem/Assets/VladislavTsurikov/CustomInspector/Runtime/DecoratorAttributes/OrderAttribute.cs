using System;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class OrderAttribute : Attribute
    {
        public OrderAttribute(int order)
        {
            Order = order;
        }

        public int Order { get; }
    }
}
