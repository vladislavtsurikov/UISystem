using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.UnityUtility.Runtime
{
    public static class GameObjectEx
    {
        public static List<GameObject> GetAllChildren(this GameObject gameObject)
        {
            Transform[] childTransforms = gameObject.GetComponentsInChildren<Transform>();
            var allChildren = new List<GameObject>(childTransforms.Length);

            foreach (Transform child in childTransforms)
            {
                if (child.gameObject != gameObject)
                {
                    allChildren.Add(child.gameObject);
                }
            }

            return allChildren;
        }

        public static List<GameObject> GetAllChildrenAndSelf(this GameObject gameObject)
        {
            Transform[] childTransforms = gameObject.GetComponentsInChildren<Transform>();
            var allChildren = new List<GameObject>(childTransforms.Length);

            foreach (Transform child in childTransforms)
            {
                allChildren.Add(child.gameObject);
            }

            return allChildren;
        }

        public static bool ContainInChildren(this GameObject gameObject, GameObject findGameObject)
        {
            Transform[] childTransforms = gameObject.GetComponentsInChildren<Transform>();

            foreach (Transform child in childTransforms)
            {
                if (GameObjectUtility.IsSameGameObject(child.gameObject, findGameObject, true))
                {
                    return true;
                }
            }

            return false;
        }

        public static Mesh GetMesh(this GameObject gameObject)
        {
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                return meshFilter.sharedMesh;
            }

            SkinnedMeshRenderer skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null && skinnedMeshRenderer.sharedMesh != null)
            {
                return skinnedMeshRenderer.sharedMesh;
            }

            return null;
        }

        public static bool IsRendererEnabled(this GameObject gameObject)
        {
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                return meshRenderer.enabled;
            }

            SkinnedMeshRenderer skinnedRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
            if (skinnedRenderer != null)
            {
                return skinnedRenderer.enabled;
            }

            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                return spriteRenderer.enabled;
            }

            return false;
        }

        public static Bounds GetInstantiatedBounds(this GameObject prefab)
        {
            GameObject go = Object.Instantiate(prefab);
            go.transform.position = prefab.transform.position;
            var bounds = new Bounds(go.transform.position, Vector3.zero);
            foreach (Renderer r in go.GetComponentsInChildren<Renderer>())
            {
                bounds.Encapsulate(r.bounds);
            }

            foreach (Collider c in go.GetComponentsInChildren<Collider>())
            {
                bounds.Encapsulate(c.bounds);
            }

            Object.DestroyImmediate(go);
            return bounds;
        }

        public static Bounds GetObjectWorldBounds(this GameObject gameObject)
        {
            var worldBounds = new Bounds();
            var found = false;

            for (var i = 0; i < gameObject.transform.childCount; i++)
            {
                GameObject go = gameObject.transform.GetChild(i).gameObject;

                if (!go.activeInHierarchy)
                {
                    continue;
                }

                Renderer renderer = go.GetComponent<Renderer>();
                SkinnedMeshRenderer skinnedMeshRenderer;
                RectTransform rectTransform;

                if (renderer != null)
                {
                    if (!found)
                    {
                        worldBounds = renderer.bounds;
                        found = true;
                    }
                    else
                    {
                        worldBounds.Encapsulate(renderer.bounds);
                    }
                }
                else if ((skinnedMeshRenderer = go.GetComponent<SkinnedMeshRenderer>()) != null)
                {
                    if (!found)
                    {
                        worldBounds = skinnedMeshRenderer.bounds;
                        found = true;
                    }
                    else
                    {
                        worldBounds.Encapsulate(skinnedMeshRenderer.bounds);
                    }
                }
                else if ((rectTransform = go.GetComponent<RectTransform>()) != null)
                {
                    var fourCorners = new Vector3[4];
                    rectTransform.GetWorldCorners(fourCorners);
                    var rectBounds = new Bounds();

                    rectBounds.center = fourCorners[0];
                    rectBounds.Encapsulate(fourCorners[1]);
                    rectBounds.Encapsulate(fourCorners[2]);
                    rectBounds.Encapsulate(fourCorners[3]);

                    if (!found)
                    {
                        worldBounds = rectBounds;
                        found = true;
                    }
                    else
                    {
                        worldBounds.Encapsulate(rectBounds);
                    }
                }
            }

            if (!found)
            {
                return new Bounds(gameObject.transform.position, Vector3.one);
            }

            return worldBounds;
        }

        public static void CopyTransform(this GameObject gameObject, GameObject copyGameObject,
            bool copyPosition = true)
        {
            if (copyPosition)
            {
                gameObject.transform.position = copyGameObject.transform.position;
            }

            gameObject.transform.rotation = copyGameObject.transform.rotation;
            gameObject.transform.localScale = copyGameObject.transform.localScale;
        }

        public static void DisableMeshRenderers(this GameObject gameObject)
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();

            if (renderer != null)
            {
                renderer.enabled = false;
            }

            for (var i = 0; i < gameObject.transform.childCount; i++)
            {
                GameObject go = gameObject.transform.GetChild(i).gameObject;

                if (!go.activeInHierarchy)
                {
                    continue;
                }

                renderer = go.GetComponent<Renderer>();

                if (renderer != null)
                {
                    renderer.enabled = false;
                }
            }
        }

        public static T InstantiateWithComponent<T>(this GameObject prefab, Transform parent = null) where T : Component
        {
            if (prefab == null)
            {
                Debug.LogError("GameObjectExtensions: Prefab is null.");
                return null;
            }

            GameObject instance = Object.Instantiate(prefab, parent);
            T component = instance.GetComponent<T>();

            if (component == null)
            {
                Debug.LogError(
                    $"GameObjectExtensions: Component of type {typeof(T).Name} not found in the instantiated prefab.");
                Object.Destroy(instance);
                return null;
            }

            return component;
        }

        public static void ClearChildren(this GameObject parent)
        {
            var children = parent.transform.childCount;
            for (var i = children - 1; i >= 0; i--)
            {
                Object.Destroy(parent.transform.GetChild(i).gameObject);
            }
        }
    }
}
