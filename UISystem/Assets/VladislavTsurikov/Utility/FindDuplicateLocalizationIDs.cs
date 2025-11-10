#if UNITY_EDITOR
#if UNITY_LOCALIZATION
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Tables;

namespace VladislavTsurikov.Utility
{
    public static class FindDuplicateLocalizationIDs
    {
        [MenuItem("Tools/Localization/Find Duplicate Localization IDs")]
        public static void FindDuplicates()
        {
            var sharedTableGuids = AssetDatabase.FindAssets("t:SharedTableData");

            var globalIdMap = new Dictionary<long, List<string>>();
            var hasDuplicates = false;

            foreach (var guid in sharedTableGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                SharedTableData sharedTableData = AssetDatabase.LoadAssetAtPath<SharedTableData>(path);

                if (sharedTableData == null)
                {
                    continue;
                }

                foreach (SharedTableData.SharedTableEntry entry in sharedTableData.Entries)
                {
                    if (!globalIdMap.ContainsKey(entry.Id))
                    {
                        globalIdMap[entry.Id] = new List<string>();
                    }

                    globalIdMap[entry.Id].Add($"{path} (key: {entry.Key})");
                }
            }

            foreach (KeyValuePair<long, List<string>> kvp in globalIdMap)
            {
                if (kvp.Value.Count > 1)
                {
                    hasDuplicates = true;
                    Debug.LogError($"Duplicate ID {kvp.Key} found in:\n{string.Join("\n", kvp.Value)}");
                }
            }

            if (!hasDuplicates)
            {
                Debug.Log("No duplicate localization IDs found.");
            }
        }
    }
}
#endif
#endif
