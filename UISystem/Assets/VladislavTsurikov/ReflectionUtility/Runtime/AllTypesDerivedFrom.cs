using System;
using System.Linq;
using UnityEditor;

namespace VladislavTsurikov.ReflectionUtility.Runtime
{
    public static class AllTypesDerivedFrom<T>
    {
        public static readonly Type[] Types;

        static AllTypesDerivedFrom()
        {
#if UNITY_EDITOR && UNITY_2019_2_OR_NEWER
            Types = TypeCache.GetTypesDerivedFrom<T>().Where(t => !t.IsAbstract).ToArray();
#else
            Types = GetAllAssemblyTypes().Where(t => t.IsSubclassOf(typeof(T))).ToArray();
#endif
        }

        private static Type[] GetAllAssemblyTypes() =>
            AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes()).ToArray();
    }
}
