using System;
using System.Collections.Generic;
using VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    public static class ResourceLoaderDescriptorFillContainer
    {
        private static Dictionary<Type, ResourceLoaderDescriptorFill> s_fills = new Dictionary<Type, ResourceLoaderDescriptorFill>();

        public static IReadOnlyDictionary<Type, ResourceLoaderDescriptorFill> Fills => s_fills;

        static ResourceLoaderDescriptorFillContainer()
        {
            Refresh();
        }

        private static void Refresh()
        {
            s_fills.Clear();

            foreach (ResourceLoaderDescriptorFill resourceLoaderDescriptorFill in ReflectionFactory.CreateAllInstances<ResourceLoaderDescriptorFill>())
            {
                s_fills.Add(resourceLoaderDescriptorFill.LoaderType, resourceLoaderDescriptorFill);
            }
        }

        public static ResourceLoaderDescriptor Get(Type resourceType)
        {
            Type current = resourceType;

            while (current != null)
            {
                if (s_fills.TryGetValue(current, out ResourceLoaderDescriptorFill fill))
                {
                    return fill.Fill(resourceType);
                }

                current = current.BaseType;
            }

            return null;
        }
    }
}
