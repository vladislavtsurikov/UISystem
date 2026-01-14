using System;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class MaxAttribute : Attribute
    {
        public MaxAttribute(float value)
        {
            Value = value;
        }

        public float Value { get; }
    }
}
