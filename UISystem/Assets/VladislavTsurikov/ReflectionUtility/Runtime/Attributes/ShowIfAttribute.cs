using System;

namespace VladislavTsurikov.ReflectionUtility
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ShowIfAttribute : Attribute
    {
        public readonly string FieldName;
        public readonly object Value;

        public ShowIfAttribute(string fieldName, object value)
        {
            FieldName = fieldName;
            Value = value;
        }
    }
}
