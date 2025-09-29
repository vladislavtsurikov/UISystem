using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using OdinSerializer;
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.ScriptableObjectUtility.Runtime
{
    public class SerializedScriptableObjectSingleton<T> : SerializedScriptableObject
        where T : SerializedScriptableObject
    {
        private static T s_Instance;

        public static T Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = GetPackage();
                }

                return s_Instance;
            }
        }

        private static T GetPackage()
        {
            LocationAssetAttribute locationAssetAttribute = GetCustomAttributes();

            if (locationAssetAttribute == null)
            {
                Debug.LogError("Location Attribute missing!");
                return null;
            }

            T scriptableObject = Resources.Load<T>(locationAssetAttribute.RelativePath);

            if (scriptableObject == null)
            {
                scriptableObject = CreateInstance<T>();
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    var locationFilePath = locationAssetAttribute.FilePath;
                    var directoryName = Path.GetDirectoryName(locationFilePath);
                    if (directoryName == null)
                    {
                        return scriptableObject;
                    }

                    Directory.CreateDirectory(directoryName);

                    AssetDatabase.CreateAsset(scriptableObject, locationAssetAttribute.FilePath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
#endif
            }

            return scriptableObject;
        }

        [CanBeNull]
        private static LocationAssetAttribute GetCustomAttributes() =>
            (LocationAssetAttribute)typeof(T).GetCustomAttribute(typeof(LocationAssetAttribute));
    }
}
