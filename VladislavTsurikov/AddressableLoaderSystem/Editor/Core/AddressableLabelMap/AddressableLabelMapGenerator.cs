#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core.AddressableLabelMap;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.AddressableLabelMap
{
    public static class AddressableLabelMapGenerator
    {
        [MenuItem("Tools/Addressables/Generate Label Map")]
        public static void Generate()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            var map = new Dictionary<Type, Dictionary<string, string>>();

            foreach (AddressableAssetGroup group in settings.groups)
            foreach (AddressableAssetEntry entry in group.entries)
            {
                Object asset = entry.MainAsset;
                if (asset == null)
                {
                    continue;
                }

                Type type = asset.GetType();
                if (!map.TryGetValue(type, out Dictionary<string, string> dict))
                {
                    map[type] = dict = new Dictionary<string, string>();
                }

                dict[entry.address] = group.name;
            }

            AddressableLabelMapAsset.Instance.SetMap(map);
            AssetDatabase.SaveAssets();

            Debug.Log("✅ AddressableLabelMap.asset updated");
        }
    }
}
#endif
