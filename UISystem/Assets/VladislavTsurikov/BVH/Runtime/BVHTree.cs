using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.BVH.Runtime
{
    public class BVHTree<TNode, TNodeData>
        where TNode : BVHNode<TNodeData>, new()
        where TNodeData : class
    {
        private const int _numChildren = 2;
        private TNode _root = new();

        public void Clear() => _root = new TNode();

        public AABB GetAABB() => new(_root.Position, _root.Size);

        public void InsertLeafNode(TNode node)
        {
            InsertLeafNodeRecurse(_root, node);
            node.MakeLeaf();
        }

        public void RemoveLeafNode(TNode node)
        {
            if (!node.IsLeaf)
            {
                return;
            }

            BVHNode<TNodeData> currentParent = node.Parent;
            node.SetParent(null);

            while (currentParent != null && currentParent != _root && currentParent.NumChildren == 0)
            {
                BVHNode<TNodeData> newParent = currentParent.Parent;
                currentParent.SetParent(null);
                currentParent = newParent;
            }

            if (currentParent != null)
            {
                currentParent.EncapsulateChildrenBottomUp();
            }
        }

        public List<BVHNodeRayHit<TNodeData>> RaycastAll(Ray ray, bool sort)
        {
            var nodeHits = new List<BVHNodeRayHit<TNodeData>>(10);
            RaycastAllRecurse(ray, _root, nodeHits);

            if (sort)
            {
                nodeHits.Sort((h0, h1) => h0.Distance.CompareTo(h1.Distance));
            }

            return nodeHits;
        }

        public List<BVHNode<TNodeData>> FindAllLeafNode()
        {
            var overlappedNodes = new List<BVHNode<TNodeData>>();
            FindAllLeafNodeRecurse(_root, overlappedNodes);
            return overlappedNodes;
        }

        private void FindAllLeafNodeRecurse(BVHNode<TNodeData> node, List<BVHNode<TNodeData>> outputNodes)
        {
            if (node.IsLeaf)
            {
                outputNodes.Add(node);
            }
            else
            {
                for (var childIndex = 0; childIndex < node.NumChildren; ++childIndex)
                {
                    FindAllLeafNodeRecurse(node.GetChild(childIndex), outputNodes);
                }
            }
        }

        public List<BVHNode<TNodeData>> OverlapBox(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation)
        {
            var overlappedNodes = new List<BVHNode<TNodeData>>();
            OverlapBoxRecurse(_root, boxCenter, boxSize, boxRotation, overlappedNodes);
            return overlappedNodes;
        }

        public List<BVHNode<TNodeData>> OverlapSphere(Vector3 sphereCenter, float sphereRadius)
        {
            var overlappedNodes = new List<BVHNode<TNodeData>>();
            OverlapSphereRecurse(_root, sphereCenter, sphereRadius, overlappedNodes);
            return overlappedNodes;
        }

        private void InsertLeafNodeRecurse(BVHNode<TNodeData> parent, BVHNode<TNodeData> node)
        {
            if (!parent.IsLeaf)
            {
                if (parent.NumChildren < _numChildren)
                {
                    node.SetParent(parent);
                    parent.EncapsulateChildrenBottomUp();
                }
                else
                {
                    BVHNode<TNodeData> closestChild = parent.FindClosestChild(node);
                    InsertLeafNodeRecurse(closestChild, node);
                }
            }
            else
            {
                var newParent = new TNode();
                BVHNode<TNodeData> oldParent = parent.Parent;

                node.SetParent(newParent);
                parent.SetParent(newParent);
                newParent.SetParent(oldParent);
                newParent.EncapsulateChildrenBottomUp();
            }
        }

        private void RaycastAllRecurse(Ray ray, BVHNode<TNodeData> node, List<BVHNodeRayHit<TNodeData>> outputNodes)
        {
            float t;
            if (node.Raycast(ray, out t))
            {
                if (node.IsLeaf)
                {
                    outputNodes.Add(new BVHNodeRayHit<TNodeData>(ray, node, t));
                }

                for (var childIndex = 0; childIndex < node.NumChildren; ++childIndex)
                {
                    RaycastAllRecurse(ray, node.GetChild(childIndex), outputNodes);
                }
            }
        }

        private void OverlapBoxRecurse(BVHNode<TNodeData> node, Vector3 boxCenter, Vector3 boxSize,
            Quaternion boxRotation, List<BVHNode<TNodeData>> outputNodes)
        {
            if (node.IntersectsBox(boxCenter, boxSize, boxRotation))
            {
                if (node.IsLeaf)
                {
                    outputNodes.Add(node);
                }
                else
                {
                    for (var childIndex = 0; childIndex < node.NumChildren; ++childIndex)
                    {
                        OverlapBoxRecurse(node.GetChild(childIndex), boxCenter, boxSize, boxRotation, outputNodes);
                    }
                }
            }
        }

        private void OverlapSphereRecurse(BVHNode<TNodeData> node, Vector3 sphereCenter, float sphereRadius,
            List<BVHNode<TNodeData>> outputNodes)
        {
            if (node.IntersectsSphere(sphereCenter, sphereRadius))
            {
                if (node.IsLeaf)
                {
                    outputNodes.Add(node);
                }
                else
                {
                    for (var childIndex = 0; childIndex < node.NumChildren; ++childIndex)
                    {
                        OverlapSphereRecurse(node.GetChild(childIndex), sphereCenter, sphereRadius, outputNodes);
                    }
                }
            }
        }

#if UNITY_EDITOR

        #region Gizmos

        private void DrawRaycastRecurse(Ray ray, BVHNode<TNodeData> node, Matrix4x4 transformMtx, Color nodeColor)
        {
            var t = 0.0f;
            if (node.Raycast(ray, out t))
            {
                GizmosEx.PushColor(nodeColor);
                var nodeMatrix = Matrix4x4.TRS(node.Position, Quaternion.identity, node.Size);
                GizmosEx.PushMatrix(transformMtx * nodeMatrix);
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                GizmosEx.PopMatrix();
                GizmosEx.PopColor();

                for (var childIndex = 0; childIndex < node.NumChildren; ++childIndex)
                {
                    DrawRaycastRecurse(ray, node.GetChild(childIndex), transformMtx, nodeColor);
                }
            }
        }

        public List<BVHNodeRayHit<TNodeData>> DrawRaycast(Ray ray, Matrix4x4 transformMtx, Color nodeColor)
        {
            List<BVHNodeRayHit<TNodeData>> nodeHits = RaycastAll(ray, false);

            DrawRaycastRecurse(ray, _root, transformMtx, nodeColor);

            return nodeHits;
        }

        public void DrawAllCells(Matrix4x4 transformMtx, Color lineColor)
        {
            GizmosEx.PushColor(lineColor);
            DrawCellRecurse(_root, transformMtx, lineColor);
            GizmosEx.PopColor();
        }

        private void DrawCellRecurse(BVHNode<TNodeData> node, Matrix4x4 transformMtx, Color lineColor)
        {
            if (_root != node)
            {
                var nodeMatrix = Matrix4x4.TRS(node.Position, Quaternion.identity, node.Size);
                GizmosEx.PushMatrix(transformMtx * nodeMatrix);
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                GizmosEx.PopMatrix();
            }

            for (var childIndex = 0; childIndex < node.NumChildren; ++childIndex)
            {
                DrawCellRecurse(node.GetChild(childIndex), transformMtx, lineColor);
            }
        }

        #endregion

#endif
    }
}
