#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create
{
    public static class EditorResourceLoaderRegistry
    {
        private static readonly List<ResourceLoaderTypeInfo> _wrappers = new();
        public static IReadOnlyList<ResourceLoaderTypeInfo> Wrappers => _wrappers;

        public static void Refresh()
        {
            _wrappers.Clear();

            Type[] loaderTypes = AllTypesDerivedFrom<ResourceLoader>.Types;

            foreach (Type loaderType in loaderTypes)
            {
                var container = new ResourceLoaderDescriptorContainer();

                var descriptor = ResourceLoaderDescriptorFillContainer.Get(loaderType);
                if (descriptor == null)
                {
                    continue;
                }

                container.ChangeResourceLoaderDescriptor(descriptor);
                container.SetActiveByBaseTypeName(descriptor.GetBaseTypeName());

                _wrappers.Add(new ResourceLoaderTypeInfo(loaderType, container));
            }
        }
    }
}
#endif
