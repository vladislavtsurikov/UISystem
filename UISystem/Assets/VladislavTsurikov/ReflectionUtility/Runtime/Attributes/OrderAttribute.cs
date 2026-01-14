using System;

namespace VladislavTsurikov.ReflectionUtility
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class OrderAttribute : Attribute
    {
        public readonly int Order;

        public OrderAttribute(int order) => Order = order;
    }
}
