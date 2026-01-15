using System;

namespace VladislavTsurikov.CustomInspector.Runtime
{
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
