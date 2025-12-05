#if UNITY_EDITOR
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov.RendererStack.Editor.Sectorize.TerrainSystem;
using VladislavTsurikov.SceneDataSystem.Editor.StreamingUtility;
using VladislavTsurikov.SceneDataSystem.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;
using VladislavTsurikov.SceneUtility.Editor;

namespace VladislavTsurikov.RendererStack.Runtime.Sectorize
{
    public partial class Sectorize
    {
        public async UniTask CreateSectors()
        {
            Scene activeScene = SceneManager.GetActiveScene();

            SceneDataManager sceneDataManager = SceneDataManagerFinder.Find(activeScene);

            sceneDataManager.SceneType = SceneType.ParentScene;

            CreateSectorsForAdditiveScenes(activeScene);

            await CreateScenesForActiveSceneTerrains();
        }

        private static void CreateSectorsForAdditiveScenes(Scene activeScene)
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                if (activeScene == scene)
                {
                    continue;
                }

                List<VirtualTerrain> terrains = AllVirtualTerrainTypes.FindAll(scene);

                if (terrains.Count == 0)
                {
                    continue;
                }

                for (var terrainIndex = 0; terrainIndex < terrains.Count; terrainIndex++)
                {
                    SectorLayerManager.Instance.AddSector(GetSectorLayerTag(), SceneAssetFinder.FindSceneAsset(scene),
                        SceneObjectsBounds.GetSceneObjectsBounds(scene, true).ToBounds());
                }
            }
        }

        private static async UniTask CreateScenesForActiveSceneTerrains()
        {
            Scene activeScene = SceneManager.GetActiveScene();

            List<VirtualTerrain> activeSceneTerrains = AllVirtualTerrainTypes.FindAll(activeScene);

            if (activeSceneTerrains.Count == 0)
            {
                return;
            }

            EditorSceneManager.MarkSceneDirty(activeScene);

            for (var i = 0; i < activeSceneTerrains.Count; i++)
            {
                var progress = i / (float)activeSceneTerrains.Count * 100;

                EditorUtility.DisplayProgressBar(
                    "Sector: " + progress + "%" + " (" + i + "/" + activeSceneTerrains.Count + ")",
                    "Running " + activeSceneTerrains[i].Target.name, progress / 100);

                await StreamingUtilityEditor.CreateScene(GetSectorLayerTag(), activeSceneTerrains[i].Target.name,
                    activeSceneTerrains[i].GetTerrainBounds(),
                    new List<GameObject> { activeSceneTerrains[i].Target.gameObject });
            }

            SceneDataManagerUtility.InstanceSceneDataManagerForAllScenes();

            foreach (Sector sector in StreamingUtility.GetAllScenes(GetSectorLayerTag()))
            {
                sector.SceneDataManager.SceneType = SceneType.Subscene;
            }

            Editor.Sectorize.Integration.SceneManagerIntegration.PrepareSceneManager();

            CreateScenesAfterEvent?.Invoke();

            EditorUtility.ClearProgressBar();
        }
    }
}
#endif
