using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.BVH.Runtime;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.QuadTree.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.Common
{
    public class BvhCellTree<T> where T : class, IHasRect
    {
        private readonly Dictionary<T, BVHNodeAABB<T>> _leafNodes = new();
        protected readonly BVHTree<BVHNodeAABB<T>, T> Tree = new();

        public void Clear()
        {
            Tree.Clear();
            _leafNodes.Clear();
        }

        public AABB GetAABB() => Tree.GetAABB();

        public BVHNodeAABB<T> RegisterObject(T cell, AABB aabb)
        {
            if (_leafNodes.ContainsKey(cell))
            {
                return null;
            }

            var treeNode = new BVHNodeAABB<T>(cell);
            treeNode.Position = aabb.Center;
            treeNode.Size = aabb.Size;
            Tree.InsertLeafNode(treeNode);
            _leafNodes.Add(cell, treeNode);

            return treeNode;
        }

        public List<T> OverlapCellsBox(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation)
        {
            var overlappedObjects = new List<T>();

            foreach (BVHNode<T> node in Tree.OverlapBox(boxCenter, boxSize, boxRotation))
            {
                overlappedObjects.Add(node.Data);
            }

            return overlappedObjects;
        }

        public List<T> OverlapCellsSphere(Vector3 sphereCenter, float sphereRadius)
        {
            var overlappedObjects = new List<T>();

            foreach (BVHNode<T> node in Tree.OverlapSphere(sphereCenter, sphereRadius))
            {
                overlappedObjects.Add(node.Data);
            }

            return overlappedObjects;
        }

        public T Raycast(Ray ray)
        {
            List<T> allObjectHits = RaycastAll(ray);
            T closestObjectHit = allObjectHits.Count != 0 ? allObjectHits[0] : null;
            return closestObjectHit;
        }

        public List<T> RaycastAll(Ray ray)
        {
            List<BVHNodeRayHit<T>> nodeHits = Tree.RaycastAll(ray, false);

            var overlappedCells = new List<T>();
            foreach (BVHNodeRayHit<T> hit in nodeHits)
            {
                overlappedCells.Add(hit.HitNode.Data);
            }

            return overlappedCells;
        }

        public BVHNodeAABB<T> ChangeNodeSize(T cell, AABB aabb)
        {
            if (!_leafNodes.ContainsKey(cell))
            {
                return null;
            }

            Tree.RemoveLeafNode(_leafNodes[cell]);
            _leafNodes.Remove(cell);
            return RegisterObject(cell, aabb);
        }

#if UNITY_EDITOR

        #region Gizmos

        public void DrawAllCells(Color nodeColor) => Tree.DrawAllCells(Matrix4x4.identity, nodeColor);

        public List<BVHNodeRayHit<T>> DrawRaycast(Ray ray, Color nodeColor) =>
            Tree.DrawRaycast(ray, Matrix4x4.identity, nodeColor);

        #endregion

#endif
    }
}
