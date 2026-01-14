using System;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ShowIfAttribute : Attribute
    {
        public ShowIfAttribute(string conditionMemberName, bool inverse = false)
        {
            ConditionMemberName = conditionMemberName;
            Inverse = inverse;
        }

        public string ConditionMemberName { get; }
        public bool Inverse { get; }
    }
}
