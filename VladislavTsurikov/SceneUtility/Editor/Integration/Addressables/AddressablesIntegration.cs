#if SCENE_MANAGER_ADDRESSABLES
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using VladislavTsurikov.SceneUtility.Editor;

namespace VladislavTsurikov.SceneUtility.ScriptsEditor.Integration.Addressables
{
    [InitializeOnLoad]
    internal static class AddressablesIntegration
    {
        static AddressablesIntegration()
        {
            Settings = AddressableAssetSettingsDefaultObject.GetSettings(true);

            if (!Application.isPlaying)
            {
                ScenesInBuildUtility.SetupScenesInBuildOverride = SetupScenesInBuild;
            }
        }

        private static AddressableAssetSettings Settings { get; }

        private static void SetupScenesInBuild(List<string> scenePaths)
        {
            ScenesInBuildUtility.ClearAllBuildScene();
            ClearAllScene();

            foreach (var path in scenePaths)
            {
                AddScene(path);
            }
        }

        internal static bool IsAdded(params string[] paths)
        {
            if (paths == null || paths.Length == 0)
            {
                return false;
            }

            if (!Settings)
            {
                return false;
            }

            IEnumerable<AddressableAssetEntry> entries =
                Settings.groups.SelectMany(g => g.entries?.Where(e => paths.Contains(e.AssetPath)));
            return paths.All(path => entries.Any(e => e.AssetPath == path));
        }

        private static void ClearAllScene()
        {
            var removeScenePaths = new List<string>();

            foreach (AddressableAssetGroup assetGroup in Settings.groups)
            {
                foreach (AddressableAssetEntry assetEntry in assetGroup.entries)
                {
                    if (assetEntry.IsScene)
                    {
                        removeScenePaths.Add(assetEntry.AssetPath);
                    }
                }
            }

            foreach (var scenePath in removeScenePaths)
            {
                RemoveScene(scenePath);
            }
        }

        private static void AddScene(string scenePath)
        {
            if (!Settings)
            {
                return;
            }

            Settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(scenePath), Settings.DefaultGroup,
                postEvent: false).SetLabel("Scene Manager", true, true);
        }

        private static void RemoveScene(string scenePath)
        {
            if (!Settings)
            {
                return;
            }

            Settings.RemoveAssetEntry(AssetDatabase.AssetPathToGUID(scenePath), false);
        }
    }
}
#endif
#endif
