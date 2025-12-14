#if UNITY_EDITOR
using System;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ResourceLoaderTemplateBaseTypeAttribute : Attribute
    {
        public Type Type { get; }

        public ResourceLoaderTemplateBaseTypeAttribute(Type type)
        {
            Type = type;
        }
    }
}
#endif
