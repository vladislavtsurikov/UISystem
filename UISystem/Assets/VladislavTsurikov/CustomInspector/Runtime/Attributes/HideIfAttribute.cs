using System;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    /// <summary>
    /// Conditionally hides a field in the inspector based on the value of another field, property, or method.
    /// The field is hidden when the condition evaluates to true.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class HideIfAttribute : Attribute
    {
        public HideIfAttribute(string conditionMemberName)
        {
            ConditionMemberName = conditionMemberName;
        }

        public string ConditionMemberName { get; }
    }
}
