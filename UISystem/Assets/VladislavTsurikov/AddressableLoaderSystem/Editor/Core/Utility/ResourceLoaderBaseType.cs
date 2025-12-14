#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    public static class ResourceLoaderBaseType
    {
        private static readonly List<Type> s_baseTypes;

        static ResourceLoaderBaseType()
        {
            HashSet<Type> types = new HashSet<Type>();

            Type[] templateTypes = AllTypesDerivedFrom<ResourceLoaderTemplate>.Types;
            for (int i = 0; i < templateTypes.Length; i++)
            {
                Type templateType = templateTypes[i];

                ResourceLoaderTemplateBaseTypeAttribute attr =
                    templateType.GetCustomAttribute<ResourceLoaderTemplateBaseTypeAttribute>(false);

                Type baseType = attr?.Type;
                if (baseType == null)
                {
                    continue;
                }

                types.Add(baseType);
            }

            s_baseTypes = types.ToList();
        }

        public static List<string> GetBaseTypeNames()
        {
            return s_baseTypes.Select(t => t.Name).ToList();
        }

        public static List<Type> GetTypes()
        {
            return s_baseTypes;
        }
    }
}
#endif
