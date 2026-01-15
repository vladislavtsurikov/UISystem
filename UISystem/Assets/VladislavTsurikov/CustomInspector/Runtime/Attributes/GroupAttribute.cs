using System;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    /// <summary>
    /// Base class for all group attributes.
    /// Groups organize fields into visual containers in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public abstract class GroupAttribute : Attribute
    {
        protected GroupAttribute(string groupPath)
        {
            GroupPath = groupPath ?? string.Empty;
        }

        /// <summary>
        /// Path of the group. Supports hierarchical groups using "/" separator.
        /// Example: "Settings/Audio" creates nested groups.
        /// </summary>
        public string GroupPath { get; }

        /// <summary>
        /// Order of the group in the inspector
        /// </summary>
        public int Order { get; set; } = 0;
    }
}
