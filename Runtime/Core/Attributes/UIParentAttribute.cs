using System;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class UIParentAttribute : Attribute
    {
        public Type ParentType { get; }
        public string ContainerId { get; }

        public UIParentAttribute(Type parentType, string containerId = null)
        {
            ParentType = parentType;
            ContainerId = containerId;
        }
    }
}
