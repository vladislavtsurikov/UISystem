using System;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class MinAttribute : Attribute
    {
        public MinAttribute(float value)
        {
            Value = value;
        }

        public float Value { get; }
    }
}
