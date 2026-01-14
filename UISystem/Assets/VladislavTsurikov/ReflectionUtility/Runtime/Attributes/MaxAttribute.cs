using System;

namespace VladislavTsurikov.ReflectionUtility
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class MaxAttribute : Attribute
    {
        public readonly float Value;

        public MaxAttribute(float value) => Value = value;
    }
}
