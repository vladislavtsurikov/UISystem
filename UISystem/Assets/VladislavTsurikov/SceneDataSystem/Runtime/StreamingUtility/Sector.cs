using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.SceneUtility.Runtime;
using GameObjectUtility = VladislavTsurikov.UnityUtility.Runtime.GameObjectUtility;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility
{
    [Serializable]
    public sealed class Sector
    {
        public Bounds Bounds;

        [OdinSerialize]
        public SceneReference SceneReference = new();

        [NonSerialized]
        private ObjectBoundsBVHTree _objectBoundsBvhTree = new();

        //References to GameObject in another SceneReference.Scene
        private List<GameObject> _references = new();

        public Sector(Scene scene, Bounds bounds, ObjectBoundsBVHTree objectBoundsBvhTree)
        {
            SceneReference = new SceneReference(scene);
            Bounds = bounds;
            Setup(objectBoundsBvhTree);
        }

#if UNITY_EDITOR
        public Sector(SceneAsset sceneAsset, Bounds bounds, ObjectBoundsBVHTree objectBoundsBvhTree)
        {
            SceneReference = new SceneReference(sceneAsset);
            Bounds = bounds;
            Setup(objectBoundsBvhTree);
        }
#endif

        public bool CachedScene => SceneReference.CachedScene;

        public SceneDataManager SceneDataManager
        {
            get
            {
                SceneDataManager sceneDataManager = null;

                _references.RemoveAll(go => go == null);

                foreach (GameObject go in _references)
                {
                    sceneDataManager = (SceneDataManager)go.GetComponentInChildren(typeof(SceneDataManager));
                }

                if (sceneDataManager == null)
                {
                    sceneDataManager =
                        (SceneDataManager)GameObjectUtility.FindObjectOfType(typeof(SceneDataManager),
                            SceneReference.Scene);

                    if (sceneDataManager == null)
                    {
                        return null;
                    }

                    _references.Add(sceneDataManager.gameObject);
                }

                return sceneDataManager;
            }
        }

        public AABB AABB => new(Bounds.center, Bounds.size);

        public bool IsLoaded => SceneReference.IsLoaded;

        public void Setup(ObjectBoundsBVHTree objectBoundsBvhTree) => _objectBoundsBvhTree = objectBoundsBvhTree;

        [OnDeserializing]
        private void OnDeserializing() => _references = new List<GameObject>();

        public void LoadScene(float waitForSeconds = 0) => SceneReference.LoadScene(waitForSeconds).Forget();

        public void UnloadScene(float waitForSeconds = 0) => SceneReference.UnloadScene(waitForSeconds).Forget();

        public void CacheScene(float waitForSeconds = 0, float keepScene = 0) =>
            SceneReference.CacheScene(waitForSeconds, keepScene);

        public void AddReference(GameObject gameObject)
        {
            if (!_references.Contains(gameObject))
            {
                _references.Add(gameObject);
            }
        }

        public List<GameObject> GetReferences() => new(_references);

        public GameObject GetReference(string gameObjectName)
        {
            List<GameObject> gameObjects = _references.FindAll(o => o.name == gameObjectName);

            if (gameObjects.Count == 0)
            {
                return null;
            }

            return gameObjects[0];
        }

        private void SetActiveGameObjects(bool setActive)
        {
            GameObject[] gameObjects = SceneReference.Scene.GetRootGameObjects();

            foreach (GameObject go in gameObjects)
            {
                go.SetActive(setActive);
            }
        }

        public void ChangeObjectBoundsNodeSize(bool markSceneDirty = true)
        {
            _objectBoundsBvhTree.ChangeNodeSize(this, SceneObjectsBounds.GetSceneObjectsBounds(this));

#if UNITY_EDITOR
            if (!Application.isPlaying && markSceneDirty)
            {
                SceneReference.MarkSceneDirty();
            }
#endif
        }

        public bool IsValid()
        {
#if UNITY_EDITOR
            return SceneReference.IsValid();
#else
            return true;
#endif
        }
    }
}
