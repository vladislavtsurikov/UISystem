#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.UIElementsUtility.Editor.Core
{
    public static class MenuUtility
    {
        [MenuItem("Tools/Vladislav Tsurikov/UIElementsUtility/Generate Default Data Group", false, 0)]
        public static void GenerateDefaultDataGroup()
        {
            foreach (Type type in TypeHierarchyHelper.GetDerivedTypes(typeof(DataGroupScriptableObjectCreator<,>)))
            {
                var dataGroupGenerator = Activator.CreateInstance(type);

                MethodInfo methodInfo = type.GetMethod("Run");
                if (methodInfo != null)
                {
                    methodInfo.Invoke(dataGroupGenerator, null);
                }
            }
        }

        [MenuItem("Tools/Vladislav Tsurikov/UIElementsUtility/Generate API", false, 0)]
        public static void GenerateAPI()
        {
            foreach (Type type in TypeHierarchyHelper.GetDerivedTypes(typeof(DataGroupAPIGenerator<,>)))
            {
                var dataGroupGenerator = Activator.CreateInstance(type);

                MethodInfo methodInfo = type.GetMethod("Run");
                if (methodInfo != null)
                {
                    methodInfo.Invoke(dataGroupGenerator, null);
                }
            }
        }
    }
}
#endif
