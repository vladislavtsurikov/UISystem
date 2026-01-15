using System;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class FoldoutGroupAttribute : GroupAttribute
    {
        public FoldoutGroupAttribute(string groupPath, bool expanded = false) : base(groupPath)
        {
            Expanded = expanded;
        }

        public bool Expanded { get; }
    }
}
