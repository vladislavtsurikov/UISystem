#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.SceneUtility.Editor
{
    public static class ScenesInBuildUtility
    {
        public delegate void SetupScenesInBuildOverrideDelegate(List<string> scenePaths);

        internal static SetupScenesInBuildOverrideDelegate SetupScenesInBuildOverride;

        public static void Setup(List<string> scenePaths)
        {
            if (Application.isPlaying)
            {
                return;
            }

            if (SetupScenesInBuildOverride != null)
            {
                SetupScenesInBuildOverride.Invoke(scenePaths);
            }
            else
            {
                SetupScenesInBuildSettings(scenePaths);
            }
        }

        private static void SetupScenesInBuildSettings(List<string> scenePaths)
        {
            ClearAllBuildScene();

            foreach (var path in scenePaths)
            {
                AddBuildScene(path);
            }
        }

        public static void ClearAllBuildScene() => EditorBuildSettings.scenes = Array.Empty<EditorBuildSettingsScene>();

        public static bool HasSceneInBuild(string pathToScene) =>
            EditorBuildSettings.scenes.Any(editorBuildSettingsScene =>
                editorBuildSettingsScene.path == pathToScene);

        public static void AddBuildScene(string pathToScene)
        {
            if (HasSceneInBuild(pathToScene))
            {
                return;
            }

            var newScene = new EditorBuildSettingsScene(pathToScene, true);
            var tempScenes = EditorBuildSettings.scenes.ToList();
            tempScenes.Add(newScene);
            EditorBuildSettings.scenes = tempScenes.ToArray();
        }

        public static void RemoveBuildScene(string pathToScene)
        {
            if (HasSceneInBuild(pathToScene))
            {
                return;
            }

            var tempScenes = EditorBuildSettings.scenes.ToList();
            tempScenes.RemoveAll(scene => scene.path == pathToScene);
            EditorBuildSettings.scenes = tempScenes.ToArray();
        }
    }
}
#endif
