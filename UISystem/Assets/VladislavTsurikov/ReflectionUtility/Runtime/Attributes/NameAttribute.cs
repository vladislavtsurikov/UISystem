using System;

namespace VladislavTsurikov.ReflectionUtility
{
    public sealed class NameAttribute : Attribute
    {
        public readonly string Name;

        public NameAttribute(string name) => Name = name;
    }
}
