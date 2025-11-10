using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.BVH.Runtime;
using VladislavTsurikov.Math.Runtime;

namespace VladislavTsurikov.ColliderSystem.Runtime
{
    public abstract class ColliderObject
    {
        public readonly object Obj;
        public PathToColliderObject PathToColliderObject;

        protected ColliderObject(object obj) => Obj = obj;

        public virtual void Raycast(Ray ray, List<RayHit> sortedObjectHits)
        {
            Mesh mesh = GetMesh();
            if (mesh != null && IsRendererEnabled())
            {
                MeshRayHit meshRayHit = GetMesh().Raycast(ray, GetMatrix());
                if (meshRayHit != null)
                {
                    sortedObjectHits.Add(new RayHit(this, meshRayHit));
                }
            }
        }

        public void SetPathToObjectCollider<T>(List<object> pathDatas, BVHObjectTree<T> tree, BVHNode<T> node)
            where T : ColliderObject
        {
            if (pathDatas == null)
            {
                return;
            }

            pathDatas.Add(tree);
            pathDatas.Add(node);

            SetPathToObjectCollider(pathDatas);
        }

        protected virtual void SetPathToObjectCollider(List<object> pathDatas)
        {
        }

        public abstract bool IsRendererEnabled();
        public abstract Mesh GetMesh();
        public abstract GameObject GetPrefab();
        public abstract Matrix4x4 GetMatrix();
        public abstract OBB GetOBB();
        public abstract AABB GetAABB();
        public abstract int GetLayer();
        public abstract bool IsValid();
    }
}
