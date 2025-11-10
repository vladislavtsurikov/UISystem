using System;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.ColliderSystem;
using VladislavTsurikov.SceneDataSystem.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;
#if UNITY_EDITOR
using VladislavTsurikov.SceneDataSystem.Editor.Utility;
#endif

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer
{
    public static class TerrainObjectRendererAPI
    {
        public static void RemoveInstances(int id)
        {
            foreach (SceneDataManager sceneDataManager in SceneDataManagerUtility.GetAllSceneDataManager())
            {
                var terrainObjectRendererData =
                    (TerrainObjectRendererData)sceneDataManager.SceneDataStack.GetElement(
                        typeof(TerrainObjectRendererData));

                terrainObjectRendererData?.RemoveInstances(id);
            }
        }

        public static void RemoveInstances(GameObject prefab) => RemoveInstances(prefab.GetInstanceID());

        public static Prototype AddMissingPrototype(GameObject go)
        {
            TerrainObjectRenderer terrainObjectRenderer = TerrainObjectRenderer.Instance;

#if UNITY_EDITOR
            if (terrainObjectRenderer == null)
            {
                SceneDataSystemUtility.SetActiveSceneAsParentSceneType();
                terrainObjectRenderer = TerrainObjectRenderer.Instance;
            }
#endif

            Prototype proto = terrainObjectRenderer.SelectionData.GetProto(go);

            if (proto == null)
            {
                proto = terrainObjectRenderer.SelectionData.AddMissingPrototype(go);
            }

            return proto;
        }

        public static void AddMissingPrototype(List<GameObject> gameObjectList)
        {
            foreach (GameObject go in gameObjectList)
            {
                AddMissingPrototype(go);
            }
        }

        public static TerrainObjectInstance AddInstance(GameObject prefab, Vector3 worldPosition, Vector3 scale,
            Quaternion rotation)
        {
            TerrainObjectRenderer terrainObjectRenderer = TerrainObjectRenderer.Instance;

#if UNITY_EDITOR
            if (terrainObjectRenderer == null)
            {
                SceneDataSystemUtility.SetActiveSceneAsParentSceneType();
                terrainObjectRenderer = TerrainObjectRenderer.Instance;
            }
#endif

            var proto = (PrototypeTerrainObject)terrainObjectRenderer.SelectionData.AddMissingPrototype(prefab);

            return AddInstance(proto, worldPosition, scale, rotation);
        }

        public static TerrainObjectInstance AddInstance(PrototypeTerrainObject proto, Vector3 position, Vector3 scale,
            Quaternion rotation)
        {
            if (proto == null)
            {
                return null;
            }

            TerrainObjectRenderer terrainObjectRenderer = TerrainObjectRenderer.Instance;

#if UNITY_EDITOR
            if (terrainObjectRenderer == null)
            {
                SceneDataSystemUtility.SetActiveSceneAsParentSceneType();
                terrainObjectRenderer = TerrainObjectRenderer.Instance;
            }
#endif

            terrainObjectRenderer.SelectionData.AddMissingPrototype(proto);

            var instance = new TerrainObjectInstance(position, scale, rotation, proto);

            return TerrainObjectRendererData.AddInstance(instance, Sectorize.Sectorize.GetSectorLayerTag());
        }

        public static List<ColliderObject> OverlapBox(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation,
            ObjectFilter objectFilter, bool quadTree = false, bool checkObbIntersection = false)
        {
            List<SceneDataManager> sceneDataManagers = SceneDataManagerFinder.OverlapBox(boxCenter, boxSize,
                boxRotation,
                Sectorize.Sectorize.GetSectorLayerTag());

            var overlappedObjects = new List<ColliderObject>();

            foreach (SceneDataManager sceneDataManager in sceneDataManagers)
            {
                if (sceneDataManager == null)
                {
                    continue;
                }

                var terrainObjectRendererData =
                    (TerrainObjectRendererData)sceneDataManager.SceneDataStack.GetElement(
                        typeof(TerrainObjectRendererData));

                if (terrainObjectRendererData != null)
                {
                    overlappedObjects.AddRange(terrainObjectRendererData.OverlapBox(boxCenter, boxSize, boxRotation,
                        objectFilter, terrainObjectRendererData, quadTree, checkObbIntersection));
                }
            }

            return overlappedObjects;
        }

        public static List<ColliderObject> OverlapSphere(Vector3 sphereCenter, float sphereRadius,
            ObjectFilter objectFilter, bool quadTree = false, bool checkObbIntersection = false)
        {
            List<SceneDataManager> sectorList =
                SceneDataManagerFinder.OverlapSphere(sphereCenter, sphereRadius,
                    Sectorize.Sectorize.GetSectorLayerTag());

            var overlappedObjects = new List<ColliderObject>();
            foreach (SceneDataManager sceneDataManager in sectorList)
            {
                if (sceneDataManager == null)
                {
                    continue;
                }

                var terrainObjectRendererData =
                    (TerrainObjectRendererData)sceneDataManager.SceneDataStack.GetElement(
                        typeof(TerrainObjectRendererData));

                if (terrainObjectRendererData != null)
                {
                    overlappedObjects.AddRange(terrainObjectRendererData.OverlapSphere(sphereCenter, sphereRadius,
                        objectFilter, terrainObjectRendererData, quadTree, checkObbIntersection));
                }
            }

            return overlappedObjects;
        }

        public static void OverlapSphere(Vector3 sphereCenter, float sphereRadius, ObjectFilter objectFilter,
            bool quadTree, bool checkObbIntersection, Func<TerrainObjectInstance, bool> func)
        {
            foreach (ColliderObject colliderObject in OverlapSphere(sphereCenter, sphereRadius, objectFilter, quadTree,
                         checkObbIntersection))
            {
                if (colliderObject == null)
                {
                    continue;
                }

                var obj = (TerrainObjectCollider)colliderObject;

                func.Invoke(obj.Instance);
            }
        }

        public static void OverlapBox(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation,
            ObjectFilter objectFilter, bool quadTree, bool checkObbIntersection, Func<TerrainObjectInstance, bool> func)
        {
            foreach (ColliderObject colliderObject in OverlapBox(boxCenter, boxSize, boxRotation, objectFilter,
                         quadTree,
                         checkObbIntersection))
            {
                if (colliderObject == null)
                {
                    continue;
                }

                var obj = (TerrainObjectCollider)colliderObject;

                if (!func.Invoke(obj.Instance))
                {
                    return;
                }
            }
        }
    }
}
