#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.SceneManagement;
using VladislavTsurikov.RendererStack.Runtime.Sectorize.SceneManagerIntegration;
using VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility;
using VladislavTsurikov.SceneManagerTool.Editor;
using VladislavTsurikov.SceneManagerTool.Runtime;
using VladislavTsurikov.SceneManagerTool.Runtime.BuildSceneCollectionSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.RendererStack.Editor.Sectorize.Integration
{
    internal static class SceneManagerIntegration
    {
        internal static void PrepareSceneManager()
        {
            List<Sector> sectors = StreamingUtility.GetAllScenes(Runtime.Sectorize.Sectorize.GetSectorLayerTag());

            if (sectors.Count == 0)
            {
                return;
            }

            if (SceneManagerData.Instance.EnableSceneManager == false)
            {
                EnableSceneManagerWithWarning();
            }

            if (SceneManagerData.Instance.EnableSceneManager)
            {
                Scene scene = SectorLayerManager.Instance.SceneDataManager.Scene;

                Profile profile = SceneManagerData.Instance.Profile;

                if (profile == null)
                {
                    SceneManagerData.Instance.Profile = Profile.CreateProfile();
                }

                DefaultBuildSceneCollection buildSceneCollection = CreateDefaultBuildSceneCollectionIfNecessary();

                SceneCollection sceneCollection = GetSectorizeSceneCollection(buildSceneCollection) ??
                                                  CreateSectorizeSceneCollection(buildSceneCollection);

                var activeSceneSettings =
                    (ActiveScene)sceneCollection.SettingsStack.CreateIfMissingType(typeof(ActiveScene));
                activeSceneSettings.SceneReference = new SceneReference(scene);

                var sectorizeStreamingScenesComponent =
                    (SectorizeStreamingScenes)sceneCollection.SceneTypeComponentStack.CreateComponentIfMissingType(
                        typeof(SectorizeStreamingScenes));

                foreach (Sector sector in
                         StreamingUtility.GetAllScenes(Runtime.Sectorize.Sectorize.GetSectorLayerTag()))
                {
                    sectorizeStreamingScenesComponent.AddSubScene(sector.SceneReference);
                }
            }
        }

        private static DefaultBuildSceneCollection CreateDefaultBuildSceneCollectionIfNecessary()
        {
            Profile profile = SceneManagerData.Instance.Profile;

            if (profile.BuildSceneCollectionStack.ElementList.Count == 0)
            {
                return (DefaultBuildSceneCollection)profile.BuildSceneCollectionStack.CreateComponent(
                    typeof(DefaultBuildSceneCollection));
            }

            if (profile.BuildSceneCollectionStack.ActiveBuildSceneCollection is DefaultBuildSceneCollection collection)
            {
                return collection;
            }

            var buildSceneCollection =
                (DefaultBuildSceneCollection)profile.BuildSceneCollectionStack.CreateComponent(
                    typeof(DefaultBuildSceneCollection));
            profile.BuildSceneCollectionStack.ActiveBuildSceneCollection = buildSceneCollection;
            return buildSceneCollection;
        }

        private static SceneCollection GetSectorizeSceneCollection(DefaultBuildSceneCollection buildSceneCollection)
        {
            Scene scene = SectorLayerManager.Instance.SceneDataManager.Scene;

            foreach (SceneCollection sceneCollection in buildSceneCollection.SceneCollectionStack.ElementList)
            {
                var activeScene = (ActiveScene)sceneCollection.SettingsStack.GetElement(typeof(ActiveScene));
                var sectorizeStreamingScenes =
                    (SectorizeStreamingScenes)sceneCollection.SceneTypeComponentStack.GetElement(
                        typeof(SectorizeStreamingScenes));

                if (activeScene == null || sectorizeStreamingScenes == null)
                {
                    continue;
                }

                if (activeScene.SceneReference.SceneName == scene.name)
                {
                    return sceneCollection;
                }
            }

            return null;
        }

        private static SceneCollection CreateSectorizeSceneCollection(DefaultBuildSceneCollection buildSceneCollection)
        {
            Scene scene = SectorLayerManager.Instance.SceneDataManager.Scene;

            SceneCollection newSceneCollection =
                buildSceneCollection.SceneCollectionStack.CreateComponent(typeof(SceneCollection));
            newSceneCollection.Name = scene.name;

            return newSceneCollection;
        }

        private static void EnableSceneManagerWithWarning()
        {
            if (EditorUtility.DisplayDialog("WARNING!",
                    "Sectorize works with the Scene Manager. If you enable the Scene Manager, this will remove your added scenes in the Build Settings and automatically add only the added scenes to the Scene Manager in the Build Settings.",
                    "OK", "Cancel"))
            {
                SceneManagerData.Instance.EnableSceneManager = true;
                SceneManagerWindow.Open();
            }
        }
    }
}
#endif
