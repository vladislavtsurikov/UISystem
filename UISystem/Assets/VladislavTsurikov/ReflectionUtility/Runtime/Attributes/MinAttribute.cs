using System;

namespace VladislavTsurikov.ReflectionUtility
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class MinAttribute : Attribute
    {
        public readonly float Value;

        public MinAttribute(float value) => Value = value;
    }
}
