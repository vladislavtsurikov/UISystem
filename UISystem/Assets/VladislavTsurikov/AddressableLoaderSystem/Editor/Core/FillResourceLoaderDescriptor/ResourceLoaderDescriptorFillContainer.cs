using System;
using System.Collections.Generic;
using VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    public static class ResourceLoaderDescriptorFillContainer
    {
        private static Dictionary<Type, ResourceLoaderDescriptor> s_resourceLoaderDescriptors = new Dictionary<Type, ResourceLoaderDescriptor>();

        public static IReadOnlyDictionary<Type, ResourceLoaderDescriptor> ResourceLoaderDescriptors => s_resourceLoaderDescriptors;

        static ResourceLoaderDescriptorFillContainer()
        {
            Refresh();
        }

        private static void Refresh()
        {
            s_resourceLoaderDescriptors.Clear();

            foreach (ResourceLoaderDescriptor resourceLoaderDescriptor in ReflectionFactory.CreateAllInstances<ResourceLoaderDescriptor>())
            {
                s_resourceLoaderDescriptors.Add(resourceLoaderDescriptor.BaseType, resourceLoaderDescriptor);
            }
        }

        public static ResourceLoaderDescriptor GetDescriptorForType(Type type)
        {
            var descriptors = ResourceLoaderDescriptors;

            while (type != null && type != typeof(object))
            {
                if (descriptors.TryGetValue(type, out var descriptor))
                    return descriptor;

                type = type.BaseType;
            }

            return null;
        }
    }
}
