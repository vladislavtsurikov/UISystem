using System;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create.ResourceLoaderDescriptorElements
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
