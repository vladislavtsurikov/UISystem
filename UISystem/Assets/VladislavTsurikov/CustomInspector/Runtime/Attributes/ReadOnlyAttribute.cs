using System;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    /// <summary>
    /// Makes a field read-only (non-editable) in the inspector.
    /// The field will be displayed but cannot be modified.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ReadOnlyAttribute : Attribute
    {
    }
}
