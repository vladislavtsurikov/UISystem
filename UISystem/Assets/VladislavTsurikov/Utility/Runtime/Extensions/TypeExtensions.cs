#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.Utility.Runtime.Extensions
{
    public static class TypeExtensions
    {
        public static string GetSourceFilePath(this Type type)
        {
            if (type == null)
            {
                return string.Empty;
            }

            return FindCsFilePath(type.Name);
        }

        public static string FindCsFilePath(string name)
        {
            var guids = AssetDatabase.FindAssets($"{name} t:Script");
            if (guids == null || guids.Length == 0)
            {
                return string.Empty;
            }

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var fileName = Path.GetFileNameWithoutExtension(path);
                if (fileName == name)
                {
                    return path;
                }
            }

            return string.Empty;
        }
    }
}
#endif
