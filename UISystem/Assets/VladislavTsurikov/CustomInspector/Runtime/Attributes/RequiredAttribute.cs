using System;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    /// <summary>
    /// Validates that a field is not null.
    /// Displays an error message in the inspector if the field is null or empty.
    /// Works with UnityEngine.Object references, strings, collections, and nullable types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class RequiredAttribute : Attribute
    {
        /// <summary>
        /// Create a Required attribute with a custom error message
        /// </summary>
        public RequiredAttribute(string message = null)
        {
            Message = message ?? "This field is required!";
        }

        public string Message { get; }
    }
}
