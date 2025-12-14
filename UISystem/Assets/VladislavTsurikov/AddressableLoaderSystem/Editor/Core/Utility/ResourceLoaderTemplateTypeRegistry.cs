#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    public static class ResourceLoaderTemplateTypeRegistry
    {
        private static readonly Dictionary<Type, Type> s_templateTypeByBaseType;
        private static readonly List<Type> s_orderedBaseTypes;

        public static IReadOnlyDictionary<Type, Type> TemplateTypesByBaseType => s_templateTypeByBaseType;

        static ResourceLoaderTemplateTypeRegistry()
        {
            s_templateTypeByBaseType = new Dictionary<Type, Type>();

            Type[] templateTypes = AllTypesDerivedFrom<ResourceLoaderTemplate>.Types;
            for (int i = 0; i < templateTypes.Length; i++)
            {
                Type templateType = templateTypes[i];

                ResourceLoaderTemplateBaseTypeAttribute attribute =
                    templateType.GetCustomAttribute<ResourceLoaderTemplateBaseTypeAttribute>(false);

                Type baseType = attribute?.Type;
                if (baseType == null)
                {
                    continue;
                }

                s_templateTypeByBaseType[baseType] = templateType;
            }

            s_orderedBaseTypes = s_templateTypeByBaseType.Keys
                .OrderBy(t => t.Name, StringComparer.Ordinal)
                .ToList();
        }

        public static ResourceLoaderTemplate CreateDefaultInstance()
        {
            if (s_orderedBaseTypes.Count == 0)
            {
                return null;
            }

            Type baseType = s_orderedBaseTypes[0];
            Type templateType;
            if (!s_templateTypeByBaseType.TryGetValue(baseType, out templateType))
            {
                return null;
            }

            return Create(templateType, null);
        }

        public static ResourceLoaderTemplate CreateByBaseTypeName(string baseTypeName)
        {
            if (string.IsNullOrEmpty(baseTypeName))
            {
                return null;
            }

            for (int i = 0; i < s_orderedBaseTypes.Count; i++)
            {
                Type baseType = s_orderedBaseTypes[i];
                if (baseType.Name != baseTypeName)
                {
                    continue;
                }

                Type templateType;
                if (!s_templateTypeByBaseType.TryGetValue(baseType, out templateType))
                {
                    return null;
                }

                return Create(templateType, null);
            }

            return null;
        }

        public static ResourceLoaderTemplate CreateByResourceType(Type resourceType)
        {
            if (resourceType == null)
            {
                return null;
            }

            Type bestBaseType = null;
            Type bestTemplateType = null;

            foreach (KeyValuePair<Type, Type> pair in s_templateTypeByBaseType)
            {
                Type baseType = pair.Key;
                if (!baseType.IsAssignableFrom(resourceType))
                {
                    continue;
                }

                if (bestBaseType == null || bestBaseType.IsAssignableFrom(baseType))
                {
                    bestBaseType = baseType;
                    bestTemplateType = pair.Value;
                }
            }

            return Create(bestTemplateType, resourceType);
        }

        public static ResourceLoaderTemplate Create(Type templateType, Type resourceType)
        {
            if (templateType == null)
            {
                return null;
            }

            if (resourceType != null)
            {
                ConstructorInfo ctorWithType = templateType.GetConstructor(new[] { typeof(Type) });
                if (ctorWithType != null && ctorWithType.IsPublic)
                {
                    return Activator.CreateInstance(templateType, resourceType) as ResourceLoaderTemplate;
                }
            }

            ConstructorInfo ctor = templateType.GetConstructor(Type.EmptyTypes);
            if (ctor == null || !ctor.IsPublic)
            {
                return null;
            }

            return Activator.CreateInstance(templateType) as ResourceLoaderTemplate;
        }
    }
}
#endif
