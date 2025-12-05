using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.ColliderSystem;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;
using GameObjectUtility = VladislavTsurikov.UnityUtility.Runtime.GameObjectUtility;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.GlobalSettings.ExtensionSystem.
    GameObjectConverter
{
    [Name("GameObject Converter")]
    public class GameObjectConverter : Extension
    {
#if UNITY_EDITOR
        private GameObject _parentObject;

        private const string ParentName = "GameObject Converter";

        public static void ConvertGameObjectToTerrainObjectRenderer()
        {
            TerrainObjectRenderer terrainObject = TerrainObjectRenderer.Instance;

#if UNITY_6000_0_OR_NEWER
            GameObject[] sceneObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
#else
            GameObject[] sceneObjects = Object.FindObjectsOfType<GameObject>();
#endif

            for (var i = 0; i < sceneObjects.Length; i++)
            {
                if (PrefabUtility.GetPrefabAssetType(sceneObjects[i]) == PrefabAssetType.NotAPrefab)
                {
                    continue;
                }

                //Find only the Parent GameObject of a prefab
                if (GameObjectUtility.GetPrefabRoot(sceneObjects[i]).name != sceneObjects[i].name)
                {
                    continue;
                }

                GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(sceneObjects[i]);

                var proto = (PrototypeTerrainObject)terrainObject.SelectionData.GetProto(prefab);

                if (proto != null)
                {
                    TerrainObjectRendererAPI.AddInstance(proto, sceneObjects[i].transform.position,
                        sceneObjects[i].transform.localScale, sceneObjects[i].transform.rotation);
                }
            }
        }

        public void ConvertTerrainObjectRendererToGameObject()
        {
            DestroyParentObject();

            var largeObjectInstances = new List<TerrainObjectCollider>();

            foreach (TerrainObjectRendererData item in
                     SceneDataStackUtility.GetAllSceneData<TerrainObjectRendererData>())
            {
                largeObjectInstances.AddRange(item.GetAllInstances());
            }

            foreach (TerrainObjectCollider instance in largeObjectInstances)
            {
                PlaceGameObject(instance.Instance.Proto, instance.Instance.Position, instance.Instance.Scale,
                    instance.Instance.Rotation);
            }
        }

        private void PlaceGameObject(PrototypeTerrainObject proto, Vector3 position, Vector3 scaleFactor,
            Quaternion rotation)
        {
            GameObject go;

#if UNITY_EDITOR
            go = PrefabUtility.InstantiatePrefab(proto.Prefab) as GameObject;

            go.transform.position = position;
            go.transform.localScale = scaleFactor;
            go.transform.rotation = rotation;

            ParentGameObject(go);
#endif
        }

        private void ParentGameObject(GameObject go)
        {
            if (_parentObject == null)
            {
                CreateParentObject();
            }

            if (_parentObject != null)
            {
                GameObject typeParentObject = _parentObject;

                if (go != null && typeParentObject != null && typeParentObject.transform != null)
                {
                    go.transform.SetParent(typeParentObject.transform, true);
                }
            }
        }

        private void CreateParentObject()
        {
            var childObject = new GameObject(ParentName);

            _parentObject = childObject.gameObject;
        }

        private void DestroyParentObject()
        {
            if (!_parentObject)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Object.Destroy(_parentObject.gameObject);
            }
            else
            {
                Object.DestroyImmediate(_parentObject.gameObject);
            }
        }
#endif
    }
}
