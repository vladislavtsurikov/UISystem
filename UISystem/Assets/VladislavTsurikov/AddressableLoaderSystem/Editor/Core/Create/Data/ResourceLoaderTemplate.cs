#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create
{
    public abstract class ResourceLoaderTemplate
    {
        public abstract Type BaseType { get; }
        public string ClassName;
        public List<BehaviorAttributeData> Behaviors = new();

        public abstract void Run();

        protected virtual void OnBuildFrom(Type loaderType)
        {
        }

        public void BuildFrom(Type loaderType)
        {
            ClassName = loaderType.Name;

            OnBuildFrom(loaderType);
        }

        public string GetBaseTypeName()
        {
            return BaseType?.Name ?? "Unknown";
        }
    }
}
#endif
