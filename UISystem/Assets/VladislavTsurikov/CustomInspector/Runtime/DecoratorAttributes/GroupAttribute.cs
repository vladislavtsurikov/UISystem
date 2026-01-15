using System;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public abstract class GroupAttribute : Attribute
    {
        protected GroupAttribute(string groupPath)
        {
            GroupPath = groupPath ?? string.Empty;
        }

        public string GroupPath { get; }

        public int Order { get; set; } = 0;
    }
}
