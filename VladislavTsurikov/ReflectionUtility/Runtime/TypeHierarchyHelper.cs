using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace VladislavTsurikov.ReflectionUtility.Runtime
{
    public static class TypeHierarchyHelper
    {
        private static readonly ConcurrentDictionary<Type, List<Type>> _typeCache = new();

        public static List<Type> GetDerivedTypes(Type baseType)
        {
            if (!baseType.IsClass)
            {
                throw new ArgumentException("Base type must be a class", nameof(baseType));
            }

            return _typeCache.GetOrAdd(baseType, _ =>
            {
#if UNITY_EDITOR && UNITY_2019_2_OR_NEWER
                IEnumerable<Type> types = TypeCache.GetTypesDerivedFrom(baseType).Where(
                    t => !t.IsAbstract
                );
#else
                var types = GetAllAssemblyTypes().Where(t => t.IsSubclassOf(baseType) && !t.IsAbstract);
#endif

                return types.ToList();
            });
        }

        private static IEnumerable<Type> GetAllAssemblyTypes()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            foreach (Type type in assembly.GetTypes())
            {
                yield return type;
            }
        }
    }
}
