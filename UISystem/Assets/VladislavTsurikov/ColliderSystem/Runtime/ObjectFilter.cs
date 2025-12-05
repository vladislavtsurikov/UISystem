using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.ColliderSystem.Runtime
{
    public class ObjectFilter
    {
        public bool FindOnlyInstancePrefabs = false;
        public bool FindOnlySpecificInstancePrefabs;
        public List<GameObject> FindPrefabs = new();
        public List<object> IgnoreObjects = new();
        public int LayerMask = ~0;

        public void ClearIgnoreObjects() => IgnoreObjects.Clear();

        public void SetIgnoreObjects(List<object> ignoreObjects)
        {
            if (ignoreObjects == null)
            {
                return;
            }

            IgnoreObjects = new List<object>(ignoreObjects);
        }

        public void SetFindPrefabs(List<GameObject> prefabs)
        {
            FindOnlySpecificInstancePrefabs = true;

            if (prefabs == null)
            {
                return;
            }

            FindPrefabs = new List<GameObject>(prefabs);
        }

        public bool IsObjectIgnored(object obj) => IgnoreObjects.Contains(obj);

        public bool Filter(int layer, GameObject prefab, object obj)
        {
            if (!LayerEx.IsLayerBitSet(LayerMask, layer) || IgnoreObjects.Contains(obj))
            {
                return false;
            }

            if (FindOnlyInstancePrefabs)
            {
                if (prefab == null)
                {
                    return false;
                }
            }

            if (FindOnlySpecificInstancePrefabs)
            {
                if (FindPrefabs.Count == 0 || prefab == null)
                {
                    return false;
                }

                foreach (GameObject findPrefab in FindPrefabs)
                {
                    if (GameObjectUtility.IsSameGameObject(findPrefab, prefab))
                    {
                        return true;
                    }
                }

                return false;
            }

            return true;
        }

        public bool Filter(ColliderObject colliderObject)
        {
            if (colliderObject == null || !colliderObject.IsValid())
            {
                return false;
            }

            if (!LayerEx.IsLayerBitSet(LayerMask, colliderObject.GetLayer()) ||
                IgnoreObjects.Contains(colliderObject.Obj))
            {
                return false;
            }

            if (FindOnlyInstancePrefabs)
            {
                if (colliderObject.GetPrefab() == null)
                {
                    return false;
                }
            }

            if (FindOnlySpecificInstancePrefabs)
            {
                if (FindPrefabs.Count == 0 || colliderObject.GetPrefab() == null)
                {
                    return false;
                }

                foreach (GameObject prefab in FindPrefabs)
                {
                    if (GameObjectUtility.IsSameGameObject(prefab, colliderObject.GetPrefab()))
                    {
                        return true;
                    }
                }

                return false;
            }

            return true;
        }
    }
}
