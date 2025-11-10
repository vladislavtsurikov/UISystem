using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace VladislavTsurikov.Core.Runtime
{
    public class BasePathFinder<T> : ScriptableObject where T : ScriptableObject
    {
        private static string s_foundPath = string.Empty;
        private static bool DebugMode => false;

        // ReSharper disable once MemberCanBeProtected.Global
        public static string Path
        {
            get
            {
#if UNITY_EDITOR
                if (!string.IsNullOrEmpty(s_foundPath))
                {
                    if (DebugMode)
                    {
                        Debug.Log($"Saved Path: {s_foundPath}");
                    }

                    return s_foundPath;
                }

                T obj = CreateInstance<T>();
                var s = MonoScript.FromScriptableObject(obj);
                var assetPath = AssetDatabase.GetAssetPath(s);
                DestroyImmediate(obj);
                var fileInfo = new FileInfo(assetPath);
                DirectoryInfo baseDir = fileInfo.Directory;
                Debug.Assert(baseDir != null, nameof(baseDir) + " != null");
                var baseDirPath = CleanPath(baseDir.ToString());
                var index = baseDirPath.IndexOf("Assets/", StringComparison.Ordinal);
                Assert.IsTrue(index >= 0);
                baseDirPath = baseDirPath.Substring(index);
                s_foundPath = baseDirPath;
                if (DebugMode)
                {
                    Debug.Log($"Found Path: {s_foundPath}");
                }

                return baseDirPath;
#else
                return "Path cannot be returned outside the Unity Editor";
#endif
            }
        }

        /// <summary> Replace any back slashes '\' with forward slashes '/' and returns the path string </summary>
        private static string CleanPath(string rawPath) => rawPath.Replace('\\', '/');
    }
}
