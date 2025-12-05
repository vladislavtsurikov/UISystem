#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.GameObjectCollider.Editor
{
    public static class GameObjectBounds
    {
        public static AABB CalcHierarchyWorldAABB(GameObject root)
        {
            AABB modelAABB = CalcHierarchyModelAABB(root);
            if (!modelAABB.IsValid)
            {
                return AABB.GetInvalid();
            }

            modelAABB.Transform(root.transform.localToWorldMatrix);
            return modelAABB;
        }

        public static OBB CalcWorldObb(GameObject gameObject)
        {
            AABB modelAABB = CalcModelAABB(gameObject);
            if (!modelAABB.IsValid)
            {
                return OBB.GetInvalid();
            }

            return new OBB(modelAABB, gameObject.transform);
        }

        public static AABB CalcWorldAABB(GameObject gameObject)
        {
            AABB modelAABB = CalcModelAABB(gameObject);
            if (!modelAABB.IsValid)
            {
                return modelAABB;
            }

            modelAABB.Transform(gameObject.transform.localToWorldMatrix);
            return modelAABB;
        }

        public static AABB CalcMeshWorldAABB(GameObject gameObject)
        {
            AABB modelAABB = CalcMeshModelAABB(gameObject);
            if (!modelAABB.IsValid)
            {
                return modelAABB;
            }

            modelAABB.Transform(gameObject.transform.localToWorldMatrix);
            return modelAABB;
        }

        public static AABB CalcHierarchyModelAABB(GameObject root)
        {
            Matrix4x4 rootTransform = root.transform.localToWorldMatrix;
            AABB finalAABB = CalcModelAABB(root);

            List<GameObject> allChildren = root.GetAllChildren();
            foreach (GameObject child in allChildren)
            {
                AABB modelAABB = CalcModelAABB(child);
                if (modelAABB.IsValid)
                {
                    // All children must have their AABBs calculated in the root local space, so we must
                    // first calculate a matrix that transforms the child object in the local space of the
                    // root. We will use this matrix to transform the child's AABB in root space.
                    Matrix4x4 rootRelativeTransform =
                        child.transform.localToWorldMatrix.GetRelativeTransform(rootTransform);
                    modelAABB.Transform(rootRelativeTransform);

                    if (finalAABB.IsValid)
                    {
                        finalAABB.Encapsulate(modelAABB);
                    }
                    else
                    {
                        finalAABB = modelAABB;
                    }
                }
            }

            return finalAABB;
        }

        public static AABB CalcMeshModelAABB(GameObject gameObject)
        {
            Mesh mesh = gameObject.GetMesh();
            if (mesh == null)
            {
                return AABB.GetInvalid();
            }

            return new AABB(mesh.bounds);
        }

        public static AABB CalcModelAABB(GameObject gameObject)
        {
            Mesh mesh = gameObject.GetMesh();
            if (mesh != null)
            {
                return new AABB(mesh.bounds);
            }

            Terrain terrain = gameObject.GetComponent<Terrain>();
            if (terrain != null)
            {
                TerrainData terrainData = terrain.terrainData;

                if (terrainData == null)
                {
                    UnityEngine.Debug.LogWarning("Terrain (" + terrain.gameObject.name +
                                                 ") not including Terrain Data");
                    return new AABB(Vector3.zero, Vector3.one);
                }

                Vector3 terrainSize = terrainData.bounds.size;
                return new AABB(terrainData.bounds.center, terrainSize);
            }

            return new AABB(Vector3.zero, Vector3.one);
        }
    }
}
#endif
