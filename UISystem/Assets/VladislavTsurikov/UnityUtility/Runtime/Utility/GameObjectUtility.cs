using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.UnityUtility.Runtime
{
    public static class GameObjectUtility
    {
        public static bool IsSameGameObject(GameObject go1, GameObject go2, bool checkID = false)
        {
            if (go1 == null || go2 == null)
            {
                return false;
            }

            if (checkID)
            {
                if (go1.GetInstanceID() != go2.GetInstanceID())
                {
                    return false;
                }

                return true;
            }

            if (go1.name != go2.name)
            {
                return false;
            }

            return true;
        }

        public static GameObject GetPrefabRoot(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return null;
            }

#if UNITY_EDITOR
            if (PrefabUtility.GetPrefabAssetType(gameObject) == PrefabAssetType.NotAPrefab)
            {
                return gameObject;
            }

            return PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject);
#else
            return gameObject;
#endif
        }

        public static void ParentGameObject(GameObject gameObject, GameObject parent)
        {
            if (gameObject != null && parent != null)
            {
                gameObject.transform.SetParent(parent.transform, true);
            }
        }

        public static GameObject FindParentGameObject(string gameObjectName, Scene scene)
        {
            GameObject container = null;

            GameObject[] sceneRoots = scene.GetRootGameObjects();
            foreach (GameObject root in sceneRoots)
            {
                if (root.name == gameObjectName)
                {
                    container = root.transform.gameObject;
                    break;
                }
            }

            if (container == null)
            {
                var childObject = new GameObject(gameObjectName);
                SceneManager.MoveGameObjectToScene(childObject, scene);
                container = childObject.transform.gameObject;
            }

            return container;
        }

        public static Object FindObjectOfType(Type type, Scene scene, bool getActiveGameObject = false)
        {
            List<Object> components = FindObjectsOfType(type, scene, getActiveGameObject);

            if (components.Count == 0)
            {
                return null;
            }

            return components[0];
        }

        public static List<Object> FindObjectsOfType(Type type, bool getActiveGameObject = false)
        {
            var objects = new List<Object>();

            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                objects.AddRange(FindObjectsOfType(type, scene, getActiveGameObject));
            }

            return objects;
        }

        public static List<Object> FindObjectsOfType(Type type, Scene scene, bool getActiveGameObject = false)
        {
            if (!scene.isLoaded)
            {
                return new List<Object>();
            }

            var componentList = new List<Object>();

            foreach (GameObject gameObject in GetSceneObjects(scene))
            {
                if (getActiveGameObject)
                {
                    if (!gameObject.activeInHierarchy)
                    {
                        continue;
                    }
                }

                componentList.AddRange(gameObject.GetComponents(type));
            }

            return componentList;
        }

        public static GameObject[] GetSceneObjects(Scene scene)
        {
            var roots = new List<GameObject>(Mathf.Max(1, scene.rootCount));
            scene.GetRootGameObjects(roots);
            var sceneObjects = new List<GameObject>(Mathf.Max(1, scene.rootCount * 5));

            foreach (GameObject root in roots)
            {
                List<GameObject> allChildrenAndSelf = root.GetAllChildrenAndSelf();
                sceneObjects.AddRange(allChildrenAndSelf);
            }

            return sceneObjects.ToArray();
        }

        public static T CopyComponent<T>(T original, GameObject destination) where T : Component
        {
            Type type = original.GetType();
            var dst = destination.GetComponent(type) as T;
            if (!dst)
            {
                dst = destination.AddComponent(type) as T;
            }

            FieldInfo[] fields = type.GetFields();
            foreach (FieldInfo field in fields)
            {
                if (field.IsStatic)
                {
                    continue;
                }

                field.SetValue(dst, field.GetValue(original));
            }

            PropertyInfo[] props = type.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                if (!prop.CanWrite || prop.Name == "name")
                {
                    continue;
                }

                prop.SetValue(dst, prop.GetValue(original, null), null);
            }

            return dst;
        }

        public static void Unspawn(List<GameObject> unspawnPrefabs)
        {
#if UNITY_6000_OR_NEWER
            GameObject[] allGameObjects = Object.FindObjectsOfType<GameObject>();
#else
            GameObject[] allGameObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
#endif

            for (var index = 0; index < allGameObjects.Length; index++)
            {
                foreach (GameObject prefab in unspawnPrefabs)
                {
                    if (IsSameGameObject(prefab, allGameObjects[index]))
                    {
                        Object.DestroyImmediate(allGameObjects[index]);
                        break;
                    }
                }
            }
        }

        public static Bounds CalculateBoundsInstantiate(GameObject go)
        {
            if (!go)
            {
                return new Bounds(Vector3.zero, Vector3.one);
            }

            GameObject originalObject = Object.Instantiate(go);
            originalObject.transform.localScale = Vector3.one;
            originalObject.hideFlags = HideFlags.DontSave;
            Bounds objectBounds = CalculateBounds(originalObject);

            if (Application.isPlaying)
            {
                Object.Destroy(originalObject);
            }
            else
            {
                Object.DestroyImmediate(originalObject);
            }

            return objectBounds;
        }

        public static Bounds CalculateBounds(GameObject go)
        {
            var combinedbounds = new Bounds(go.transform.position, Vector3.zero);
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                if (renderer is SkinnedMeshRenderer)
                {
                    var skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
                    var mesh = new Mesh();
                    skinnedMeshRenderer.BakeMesh(mesh);
                    Vector3[] vertices = mesh.vertices;

                    for (var i = 0; i <= vertices.Length - 1; i++)
                    {
                        vertices[i] = skinnedMeshRenderer.transform.TransformPoint(vertices[i]);
                    }

                    mesh.vertices = vertices;
                    mesh.RecalculateBounds();
                    Bounds meshBounds = mesh.bounds;
                    combinedbounds.Encapsulate(meshBounds);
                }
                else
                {
                    combinedbounds.Encapsulate(renderer.bounds);
                }
            }

            return combinedbounds;
        }

        public static int GetLODCount(GameObject go)
        {
            LODGroup lodGroup = go.GetComponentInChildren<LODGroup>();
            return lodGroup ? lodGroup.GetLODs().Length : 1;
        }

        private static GameObject SelectMesh(GameObject go, int lodIndex)
        {
            LODGroup lodGroup = go.GetComponentInChildren<LODGroup>();
            if (lodGroup)
            {
                LOD[] lods = lodGroup.GetLODs();

                lodIndex = Mathf.Clamp(lodIndex, 0, lods.Length - 1);

                LOD lod = lods[lodIndex];
                if (lod.renderers.Length > 0)
                {
                    if (lod.renderers[0].gameObject.GetComponent<BillboardRenderer>())
                    {
                        if (lodIndex > 0)
                        {
                            lod = lods[lodIndex - 1];
                        }
                        else
                        {
                            return null;
                        }
                    }
                }

                return lod.renderers.Length > 0 ? lod.renderers[0].gameObject : null;
            }

            MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
            if (meshRenderer)
            {
                return meshRenderer.gameObject;
            }

            meshRenderer = go.GetComponentInChildren<MeshRenderer>();
            if (meshRenderer)
            {
                return meshRenderer.gameObject;
            }

            return null;
        }
    }
}
