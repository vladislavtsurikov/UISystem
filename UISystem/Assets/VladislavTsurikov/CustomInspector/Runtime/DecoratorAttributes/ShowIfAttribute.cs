using System;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ShowIfAttribute : Attribute
    {
        public string ConditionMemberName { get; }
        public bool Value { get; }

        public ShowIfAttribute(string conditionMemberName, bool value)
        {
            ConditionMemberName = conditionMemberName;
            Value = value;
        }
    }
}
