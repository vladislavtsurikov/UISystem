#if UNITY_EDITOR
#if ADDRESSABLE_LOADER_SYSTEM_ADDRESSABLES
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Build.AnalyzeRules;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace VladislavTsurikov.AddressablesEditorTools.Editor.AnalyzeTools
{
    [InitializeOnLoad]
    public static class CustomAnalyzeRuleRegistration
    {
        static CustomAnalyzeRuleRegistration()
        {
            AnalyzeSystem.RegisterNewRule<CheckDuplicateAddressKeys>();
        }
    }

    public class CheckDuplicateAddressKeys : AnalyzeRule
    {
        private readonly List<AddressableAssetEntry> _duplicates = new List<AddressableAssetEntry>();

        public override string ruleName => "Check Duplicate Addressable Keys";

        public override bool CanFix => true;

        public override List<AnalyzeResult> RefreshAnalysis(AddressableAssetSettings settings)
        {
            List<AnalyzeResult> results = new List<AnalyzeResult>();
            _duplicates.Clear();

            Dictionary<string, AddressableAssetEntry> seenGuids = new Dictionary<string, AddressableAssetEntry>();
            List<AddressableAssetEntry> allEntries = new List<AddressableAssetEntry>();
            settings.GetAllAssets(allEntries, includeSubObjects: false);

            foreach (AddressableAssetEntry entry in allEntries)
            {
                if (string.IsNullOrEmpty(entry.guid))
                {
                    continue;
                }

                if (seenGuids.TryGetValue(entry.guid, out AddressableAssetEntry existing))
                {
                    _duplicates.Add(entry);

                    string message = "Duplicate key (GUID): " + entry.guid +
                                     "\n - " + existing.AssetPath + " (group: " + existing.parentGroup?.Name + ")" +
                                     "\n - " + entry.AssetPath + " (group: " + entry.parentGroup?.Name + ")";

                    AnalyzeResult result = new AnalyzeResult
                    {
                        resultName = message,
                        severity = MessageType.Error
                    };

                    results.Add(result);
                }
                else
                {
                    seenGuids[entry.guid] = entry;
                }
            }

            if (_duplicates.Count == 0)
            {
                results.Add(new AnalyzeResult { resultName = "No duplicate keys (GUID) found." });
            }

            return results;
        }

        public override void FixIssues(AddressableAssetSettings settings)
        {
            foreach (AddressableAssetEntry entry in _duplicates)
            {
                if (entry.parentGroup != null)
                {
                    entry.parentGroup.RemoveAssetEntry(entry);
                    Debug.Log("Removed duplicate entry: " + entry.address + " (" + entry.AssetPath + ")");
                }
            }

            AssetDatabase.SaveAssets();
        }
    }
}
#endif
#endif
