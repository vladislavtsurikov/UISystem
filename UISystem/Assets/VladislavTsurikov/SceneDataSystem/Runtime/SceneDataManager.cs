using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace VladislavTsurikov.SceneDataSystem.Runtime
{
    [ExecuteInEditMode]
    public class SceneDataManager : SerializedMonoBehaviour
    {
        [OdinSerialize]
        private SceneType _sceneType;

        [NonSerialized]
        private Sector _sector;

        public SceneDataStack SceneDataStack = new();

        public SceneType SceneType
        {
            get { return _sceneType; }
            set
            {
                if (_sceneType != value)
                {
                    _sceneType = value;
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        EditorSceneManager.MarkSceneDirty(Scene);
                    }
#endif
                }
            }
        }

        public Scene Scene => gameObject.scene;

        public bool IsSetup { get; private set; }

        public Sector Sector
        {
            get
            {
                if (SceneManager.sceneCount == 1)
                {
                    return null;
                }

                if (_sector == null)
                {
                    SectorLayerManager sectorLayerManager = SectorLayerManager.Instance;

                    if (sectorLayerManager == null)
                    {
                        return null;
                    }

                    sectorLayerManager.Setup();

                    foreach (SectorLayer sectorLayer in sectorLayerManager.SectorLayerList)
                    foreach (Sector sector in sectorLayer.SectorList)
                    {
                        if (sector.SceneReference.SceneName == Scene.name)
                        {
                            _sector = sector;
                            break;
                        }
                    }
                }

                return _sector;
            }
        }

        private void Awake() => transform.SetSiblingIndex(0);

        private void LateUpdate()
        {
            if (!gameObject.scene.isLoaded)
            {
                return;
            }

            if (!IsSetup)
            {
                Setup();
            }

            for (var i = 0; i < SceneDataStack.ElementList.Count; i++)
            {
                SceneDataStack.ElementList[i]?.LateUpdate();
            }
        }

        private void OnEnable() => Setup(true);

        private void OnDisable()
        {
            IsSetup = false;

            SceneDataStack.OnDisable();
        }

        protected override void OnAfterDeserialize()
        {
            if (SceneDataStack == null)
            {
                SceneDataStack = new SceneDataStack();
            }
        }

        public void Setup(bool forceSetup = false, CancellationToken cancellationToken = default)
        {
            if (!gameObject.scene.isLoaded || !gameObject.activeInHierarchy)
            {
                return;
            }

#if UNITY_EDITOR
            if (RemoveMultipleSceneDataManagers())
            {
                return;
            }
#endif

            cancellationToken.ThrowIfCancellationRequested();

            IsSetup = true;

            SceneDataStack.Setup(forceSetup, new object[] { this });
        }

        public bool RemoveMultipleSceneDataManagers()
        {
            GameObject[] sceneRoots = Scene.GetRootGameObjects();
            var sceneDataManagers = new List<GameObject>();
            foreach (GameObject root in sceneRoots)
            {
                if (root.name == "Scene Data Manager")
                {
                    sceneDataManagers.Add(root);
                }
            }

            var happenedDestroy = false;

            if (sceneDataManagers.Count > 1)
            {
                for (var i = sceneDataManagers.Count - 1; i >= 0; i--)
                {
                    if (i == 0)
                    {
                        break;
                    }

                    DestroyImmediate(sceneDataManagers[i].gameObject);
                    happenedDestroy = true;
                }
            }

            return happenedDestroy;
        }
    }
}
