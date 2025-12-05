using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.BVH.Runtime;
using VladislavTsurikov.Math.Runtime;

namespace VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility
{
    public class ObjectBoundsBVHTree
    {
        private readonly Dictionary<Sector, BVHNodeAABB<Sector>> _leafNodes = new();
        private readonly BVHTree<BVHNodeAABB<Sector>, Sector> _tree = new();

        public void Clear()
        {
            _tree.Clear();
            _leafNodes.Clear();
        }

        public void RegisterSector(Sector sector, AABB aabb)
        {
            if (_leafNodes.ContainsKey(sector))
            {
                return;
            }

            if (aabb.Size == Vector3.zero)
            {
                return;
            }

            var treeNode = new BVHNodeAABB<Sector>(sector);
            treeNode.Position = aabb.Center;
            treeNode.Size = aabb.Size;
            _tree.InsertLeafNode(treeNode);
            _leafNodes.Add(sector, treeNode);
        }

        public void RegisterSector(Sector sector) =>
            RegisterSector(sector, new AABB(sector.Bounds.center, sector.Bounds.size));

        public void RemoveNodes(Sector sector)
        {
            if (!_leafNodes.ContainsKey(sector))
            {
                return;
            }

            _tree.RemoveLeafNode(_leafNodes[sector]);
            _leafNodes.Remove(sector);
        }

        public List<Sector> OverlapPosition(Vector3 position)
        {
            if (_leafNodes.Count == 0)
            {
                return new List<Sector>();
            }

            return GetCurrentOverlapScenes(_tree.OverlapBox(position, Vector3.one, Quaternion.identity));
        }

        public List<Sector> OverlapBox(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation)
        {
            if (_leafNodes.Count == 0)
            {
                return new List<Sector>();
            }

            return GetCurrentOverlapScenes(_tree.OverlapBox(boxCenter, boxSize, boxRotation));
        }

        public List<Sector> OverlapSphere(Vector3 sphereCenter, float sphereRadius)
        {
            if (_leafNodes.Count == 0)
            {
                return new List<Sector>();
            }

            return GetCurrentOverlapScenes(_tree.OverlapSphere(sphereCenter, sphereRadius));
        }

        public List<Sector> RaycastAll(Ray ray)
        {
            if (_leafNodes.Count == 0)
            {
                return new List<Sector>();
            }

            List<BVHNodeRayHit<Sector>> nodeHits = _tree.RaycastAll(ray, false);

            var overlappedObjects = new List<Sector>();
            foreach (BVHNodeRayHit<Sector> hit in nodeHits)
            {
                overlappedObjects.Add(hit.HitNode.Data);
            }

            return overlappedObjects;
        }

        private List<Sector> GetCurrentOverlapScenes(List<BVHNode<Sector>> overlappedNodes)
        {
            if (_leafNodes.Count == 0)
            {
                return new List<Sector>();
            }

            var overlappedObjects = new List<Sector>();
            foreach (BVHNode<Sector> node in overlappedNodes)
            {
                Sector sector = node.Data;

                if (sector.SceneDataManager != null && sector.SceneDataManager.IsSetup)
                {
                    overlappedObjects.Add(node.Data);
                }
            }

            return overlappedObjects;
        }

        public void ChangeNodeSize(Sector sector, AABB aabb)
        {
            if (!_leafNodes.ContainsKey(sector))
            {
                return;
            }

            _tree.RemoveLeafNode(_leafNodes[sector]);
            _leafNodes.Remove(sector);
            RegisterSector(sector, aabb);
        }

        public AABB GetAABB(Sector sector)
        {
            if (!_leafNodes.ContainsKey(sector))
            {
                return AABB.GetInvalid();
            }

            BVHNodeAABB<Sector> node = _leafNodes[sector];
            var worldAABB = new AABB(node.Position, node.Size);

            return worldAABB;
        }

#if UNITY_EDITOR
        public void DrawAllCells(Color nodeColor) => _tree.DrawAllCells(Matrix4x4.identity, nodeColor);

        public List<BVHNodeRayHit<Sector>> DrawRaycast(Ray ray, Color nodeColor) =>
            _tree.DrawRaycast(ray, Matrix4x4.identity, nodeColor);
#endif
    }
}
