using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov.SceneUtility.Editor;
using VladislavTsurikov.SceneUtility.Runtime;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility
{
    [Serializable]
    public class SectorLayer
    {
        public string Tag;

        [OdinSerialize]
        public List<Sector> SectorList = new();

        public SectorLayer(string tag) => Tag = tag;

        public SectorBVHTree SectorBvhTree { get; private set; } = new();

        public ObjectBoundsBVHTree ObjectBoundsBVHTree { get; private set; } = new();

        [OnDeserializing]
        private void Initialize()
        {
            SectorBvhTree = new SectorBVHTree();
            ObjectBoundsBVHTree = new ObjectBoundsBVHTree();
        }

        public void Setup()
        {
            SectorList.RemoveAll(sector => sector == null || !sector.IsValid());

            SectorBvhTree.Clear();
            ObjectBoundsBVHTree.Clear();

            foreach (Sector sector in SectorList)
            {
                sector.Setup(ObjectBoundsBVHTree);

                SectorBvhTree.RegisterSector(sector);
                ObjectBoundsBVHTree.RegisterSector(sector, SceneObjectsBounds.GetSceneObjectsBounds(sector));
            }
        }

        public void RemoveNullScene()
        {
            for (var i = SectorList.Count - 1; i >= 0; i--)
            {
                if (SectorList[i] == null || !SectorList[i].IsValid())
                {
                    SectorList.RemoveAt(i);
                }
            }
        }

        public static void ChangeObjectBoundsNodeSize(Scene scene)
        {
            foreach (SectorLayer sectorLayer in SectorLayerManager.Instance.SectorLayerList)
            foreach (Sector sector in sectorLayer.SectorList)
            {
                if (sector.SceneReference.SceneName == scene.name)
                {
                    sectorLayer.ObjectBoundsBVHTree.ChangeNodeSize(sector,
                        SceneObjectsBounds.GetSceneObjectsBounds(sector));
                }
            }
        }

        public List<Sector> GetLoadedScenes()
        {
            var loadedScene = new List<Sector>();

            foreach (Sector sector in SectorList)
            {
                if (sector.IsLoaded)
                {
                    loadedScene.Add(sector);
                }
            }

            return loadedScene;
        }

        public static List<SectorLayer> GetCurrentSectorLayers(string tag = null)
        {
            if (tag != null)
            {
                SectorLayer sectorLayer = SectorLayerManager.Instance.GetSectorLayer(tag);

                if (sectorLayer == null)
                {
                    return null;
                }

                return new List<SectorLayer> { sectorLayer };
            }

            return SectorLayerManager.Instance.SectorLayerList;
        }

        private void RemoveSector(Sector sector)
        {
            if (SectorList.Remove(sector))
            {
                SectorBvhTree.RemoveNodes(sector);
            }
        }

#if UNITY_EDITOR
        private bool HasSceneAsset(SceneAsset sceneAsset)
        {
            foreach (Sector sector in SectorList)
            {
                if (sector.SceneReference.SceneAsset == sceneAsset)
                {
                    return true;
                }
            }

            return false;
        }

        public Sector AddSector(SceneAsset sceneAsset, Bounds bounds)
        {
            if (HasSceneAsset(sceneAsset))
            {
                return null;
            }

            var sector = new Sector(sceneAsset, bounds, ObjectBoundsBVHTree);
            SectorList.Add(sector);
            SectorBvhTree.RegisterSector(sector);

            return sector;
        }

        public SceneReference CreateScene(string sceneName, Bounds bounds)
        {
            Scene activeScene = SceneManager.GetActiveScene();
            var pathToFolder = "Assets/Scenes/Streaming Utility/" + activeScene.name + "/" + Tag;

            SceneAsset sceneAsset = SceneCreationUtility.CreateScene(sceneName, pathToFolder, true);

            return AddSector(sceneAsset, bounds).SceneReference;
        }

        public void DeleteScene(Sector sector)
        {
            if (!SectorList.Contains(sector))
            {
                return;
            }

            RemoveSector(sector);

            Scene activeScene = SceneManager.GetActiveScene();

            var roots = new List<GameObject>(Mathf.Max(1, sector.SceneReference.Scene.rootCount));
            sector.SceneReference.Scene.GetRootGameObjects(roots);

            foreach (GameObject gameObject in roots)
            {
                if (gameObject.GetComponent(typeof(SceneDataManager)) != null)
                {
                    continue;
                }

                SceneManager.MoveGameObjectToScene(gameObject, activeScene);
            }

            Async().Forget();

            async UniTask Async()
            {
                await sector.SceneReference.UnloadScene();
                sector.SceneReference.DeleteAsset();
            }
        }
#endif
    }
}
