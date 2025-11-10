using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Callbacks;

namespace VladislavTsurikov.UnityUtility.Runtime
{
    public static class FindRenderPipelineUtility
    {
        private const string HDRP_PACKAGE = "HDRenderPipelineAsset";
        private const string URP_PACKAGE = "UniversalRenderPipelineAsset";

        public static bool IsHDRP;
        public static bool IsURP;
        public static bool IsStandardRP;

#if UNITY_EDITOR
        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            IsHDRP = DoesTypeExist(HDRP_PACKAGE);
            IsURP = DoesTypeExist(URP_PACKAGE);

            if (!(IsHDRP || IsURP))
            {
                IsStandardRP = true;
            }
        }

        private static bool DoesTypeExist(string className)
        {
            Type foundType = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in GetTypesSafe(assembly)
                where type.Name == className
                select type).FirstOrDefault();

            return foundType != null;
        }

        private static IEnumerable<Type> GetTypesSafe(Assembly assembly)
        {
            Type[] types;

            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types;
            }

            return types.Where(x => x != null);
        }
#endif
    }
}
