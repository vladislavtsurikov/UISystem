using System;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    /// <summary>
    /// Groups fields into a visual box with an optional title.
    /// Useful for organizing related settings together.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class BoxGroupAttribute : GroupAttribute
    {
        /// <summary>
        /// Create a box group
        /// </summary>
        /// <param name="groupPath">Path of the group (e.g., "Settings" or "Settings/Audio")</param>
        /// <param name="showLabel">Whether to show the group title</param>
        public BoxGroupAttribute(string groupPath, bool showLabel = true) : base(groupPath)
        {
            ShowLabel = showLabel;
        }

        public bool ShowLabel { get; }
    }
}
