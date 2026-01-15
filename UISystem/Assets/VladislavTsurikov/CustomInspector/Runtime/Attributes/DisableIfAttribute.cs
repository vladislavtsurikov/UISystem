using System;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    /// <summary>
    /// Conditionally disables (makes read-only) a field in the inspector based on the value of another field, property, or method.
    /// The field is disabled when the condition evaluates to true.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class DisableIfAttribute : Attribute
    {
        public DisableIfAttribute(string conditionMemberName)
        {
            ConditionMemberName = conditionMemberName;
        }

        public string ConditionMemberName { get; }
    }
}
