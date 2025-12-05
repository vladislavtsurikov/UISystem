#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.UnityUtility.Runtime;
using Mesh = VladislavTsurikov.ColliderSystem.Runtime.Mesh;

namespace VladislavTsurikov.GameObjectCollider.Editor
{
    public class BVHGameObject : ColliderObject
    {
        private readonly GameObject _prefab;

        public BVHGameObject(GameObject gameObject, GameObject prefab) : base(gameObject) => _prefab = prefab;

        public GameObject GameObject => (GameObject)Obj;

        public override bool IsRendererEnabled()
        {
            if (GameObject == null || !GameObject.activeInHierarchy)
            {
                return false;
            }

            UnityEngine.Mesh mesh = GameObject.GetMesh();
            if (mesh != null && !GameObject.IsRendererEnabled())
            {
                return false;
            }

            return true;
        }

        public override OBB GetOBB() => GameObjectBounds.CalcWorldObb(GameObject);

        public override AABB GetAABB() => GameObjectBounds.CalcWorldAABB(GameObject);

        public override Matrix4x4 GetMatrix() => GameObject.transform.localToWorldMatrix;

        public override Mesh GetMesh()
        {
            UnityEngine.Mesh mesh = GameObject.GetMesh();
            if (mesh != null)
            {
                return MeshStack.GetEditorMesh(mesh);
            }

            return null;
        }

        public override int GetLayer() => GameObject.layer;

        public override bool IsValid() => GameObject != null;

        public override GameObject GetPrefab() => _prefab;

        public override void Raycast(Ray ray, List<RayHit> sortedObjectHits)
        {
            UnityEngine.Mesh mesh = GameObject.GetMesh();
            if (mesh != null)
            {
                if (GameObject.IsRendererEnabled())
                {
                    MeshRayHit meshRayHit = GetMesh().Raycast(ray, GameObject.transform.localToWorldMatrix);
                    if (meshRayHit != null)
                    {
                        sortedObjectHits.Add(new RayHit(GameObject, meshRayHit));
                    }
                }

                return;
            }

            TerrainCollider terrainCollider = GameObject.GetComponent<TerrainCollider>();
            if (terrainCollider != null)
            {
                if (terrainCollider.Raycast(ray, out RaycastHit raycastHit, float.MaxValue))
                {
                    sortedObjectHits.Add(new RayHit(GameObject, raycastHit.normal, raycastHit.point,
                        raycastHit.distance));
                }
            }
        }
    }
}
#endif
