#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create
{
    public abstract class ResourceLoaderDescriptor
    {
        public string ClassName;
        public abstract Type BaseType { get; }

        public List<BehaviorAttributeData> Behaviors = new();

        public abstract void Run();

        public string GetBaseTypeName()
        {
            return BaseType?.Name ?? "Unknown";
        }
    }
}
#endif
