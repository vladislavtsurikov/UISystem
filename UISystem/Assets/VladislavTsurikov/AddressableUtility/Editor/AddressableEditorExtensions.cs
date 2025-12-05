#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.AddressableUtility.Editor
{
    public static class AddressableEditorExtensions
    {
        public static Object FindAssetByAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return null;
            }

            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings != null)
            {
                AddressableAssetEntry entry = settings.FindAssetEntry(address);
                if (entry != null && !string.IsNullOrEmpty(entry.AssetPath))
                {
                    Object asset = AssetDatabase.LoadAssetAtPath<Object>(entry.AssetPath);
                    if (asset != null)
                    {
                        return asset;
                    }
                }
            }

            var guids = AssetDatabase.FindAssets(address);
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.EndsWith(".asset") || path.EndsWith(".prefab") || path.EndsWith(".png") ||
                    path.EndsWith(".mat"))
                {
                    Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                    if (obj != null)
                    {
                        return obj;
                    }
                }
            }

            return null;
        }
    }
}
#endif
