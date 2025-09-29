using System;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ParentUIHandlerAttribute : Attribute
    {
        public ParentUIHandlerAttribute(Type parentType) => ParentType = parentType;
        public Type ParentType { get; }
    }
}
