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
            if (resourceType == null)
            {
                return null;
            }

            if (s_fills.TryGetValue(resourceType, out ResourceLoaderDescriptorFill fill))
            {
                return fill.Fill(resourceType);
            }

            Type baseType = resourceType.BaseType;
            if (baseType == null)
            {
                return null;
            }

            if (s_fills.TryGetValue(baseType, out fill))
            {
                return fill.Fill(resourceType);
            }

            return null;
        }
    }
}
