using System;
using System.Collections.Generic;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core
{
    public static class ResourceLoaderTypeRegistry
    {
        private static readonly Dictionary<string, Type> _typeMap = new();

        static ResourceLoaderTypeRegistry()
        {
            var loaderTypes = AllTypesDerivedFrom<ResourceLoader>.Types;
            foreach (var type in loaderTypes)
            {
                if (!type.IsAbstract)
                {
                    _typeMap[type.Name] = type;
                }
            }
        }

        public static Type GetTypeByName(string typeName)
        {
            _typeMap.TryGetValue(typeName, out var result);
            return result;
        }
    }
}
