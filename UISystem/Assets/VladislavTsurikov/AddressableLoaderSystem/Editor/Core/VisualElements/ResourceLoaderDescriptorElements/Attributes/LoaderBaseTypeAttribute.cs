using System;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LoaderBaseTypeAttribute : Attribute
    {
        public Type Type { get; }

        public LoaderBaseTypeAttribute(Type type)
        {
            Type = type;
        }
    }
}
