using System;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    /// <summary>
    /// Groups fields into a collapsible foldout.
    /// Useful for hiding advanced or rarely-used settings.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class FoldoutGroupAttribute : GroupAttribute
    {
        /// <summary>
        /// Create a foldout group
        /// </summary>
        /// <param name="groupPath">Path of the group (e.g., "Advanced" or "Settings/Advanced")</param>
        /// <param name="expanded">Whether the foldout is expanded by default</param>
        public FoldoutGroupAttribute(string groupPath, bool expanded = false) : base(groupPath)
        {
            Expanded = expanded;
        }

        public bool Expanded { get; }
    }
}
