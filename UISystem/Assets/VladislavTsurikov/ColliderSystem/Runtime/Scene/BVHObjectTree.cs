using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.BVH.Runtime;
using VladislavTsurikov.Math.Runtime;

namespace VladislavTsurikov.ColliderSystem.Runtime
{
    public class BVHObjectTree<T> where T : ColliderObject
    {
        public readonly BVHTree<BVHNodeAABB<T>, T> Tree = new();

        public List<T> FindAllInstance(List<object> pathDatas = null)
        {
            List<BVHNode<T>> overlappedNodes = Tree.FindAllLeafNode();
            if (overlappedNodes.Count == 0)
            {
                return new List<T>();
            }

            var overlappedObjects = new List<T>();

            foreach (BVHNode<T> node in overlappedNodes)
            {
                node.Data.SetPathToObjectCollider(pathDatas, this, node);
                overlappedObjects.Add(node.Data);
            }

            return overlappedObjects;
        }

        public List<T> OverlapBox(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation, ObjectFilter objectFilter,
            bool checkOBBIntersection = false, List<object> pathDatas = null)
        {
            List<BVHNode<T>> overlappedNodes = Tree.OverlapBox(boxCenter, boxSize, boxRotation);
            if (overlappedNodes.Count == 0)
            {
                return new List<T>();
            }

            var overlappedObjects = new List<T>();

            foreach (BVHNode<T> node in overlappedNodes)
            {
                if (!node.Data.IsValid())
                {
                    continue;
                }

                if (objectFilter != null)
                {
                    if (!objectFilter.Filter(node.Data))
                    {
                        continue;
                    }
                }

                if (checkOBBIntersection)
                {
                    if (node.Data.IsRendererEnabled())
                    {
                        OBB worldOBB = node.Data.GetOBB();

                        if (BoxMath.BoxIntersectsBox(worldOBB.Center, worldOBB.Size, worldOBB.Rotation, boxCenter,
                                boxSize, boxRotation))
                        {
                            node.Data.SetPathToObjectCollider(pathDatas, this, node);
                            overlappedObjects.Add(node.Data);
                        }
                    }
                }
                else
                {
                    node.Data.SetPathToObjectCollider(pathDatas, this, node);
                    overlappedObjects.Add(node.Data);
                }
            }

            return overlappedObjects;
        }

        public List<T> OverlapSphere(Vector3 sphereCenter, float sphereRadius, ObjectFilter objectFilter,
            bool checkOBBIntersection = false, List<object> pathDatas = null)
        {
            List<BVHNode<T>> overlappedNodes = Tree.OverlapSphere(sphereCenter, sphereRadius);
            if (overlappedNodes.Count == 0)
            {
                return new List<T>();
            }

            var radiusSqr = sphereRadius * sphereRadius;
            var overlappedObjects = new List<T>();

            foreach (BVHNode<T> node in overlappedNodes)
            {
                if (objectFilter != null)
                {
                    if (!objectFilter.Filter(node.Data))
                    {
                        continue;
                    }
                }

                if (checkOBBIntersection)
                {
                    if (node.Data.IsRendererEnabled())
                    {
                        OBB worldOBB = node.Data.GetOBB();

                        Vector3 closestPt = BoxMath.CalcBoxPtClosestToPt(sphereCenter, worldOBB.Center, worldOBB.Size,
                            worldOBB.Rotation);
                        if ((closestPt - sphereCenter).sqrMagnitude <= radiusSqr)
                        {
                            node.Data.SetPathToObjectCollider(pathDatas, this, node);
                            overlappedObjects.Add(node.Data);
                        }
                    }
                }
                else
                {
                    if (Contains(node.Data.GetMatrix().GetTranslation(), sphereCenter, sphereRadius))
                    {
                        node.Data.SetPathToObjectCollider(pathDatas, this, node);
                        overlappedObjects.Add(node.Data);
                    }
                }
            }

            return overlappedObjects;
        }

        public bool Contains(Vector3 point, Vector3 Center, float Radius) => Vector3.Distance(point, Center) <= Radius;

        public RayHit Raycast(Ray ray, ObjectFilter raycastFilter, List<object> pathDatas = null)
        {
            List<RayHit> allObjectHits = RaycastAll(ray, raycastFilter);
            RayHit closestObjectHit = allObjectHits.Count != 0 ? allObjectHits[0] : null;

            return closestObjectHit;
        }

        public List<RayHit> RaycastAll(Ray ray, ObjectFilter objectFilter, List<object> pathDatas = null)
        {
            List<BVHNodeRayHit<T>> nodeHits = Tree.RaycastAll(ray, false);
            if (nodeHits.Count == 0)
            {
                return new List<RayHit>();
            }

            var sortedObjectHits = new List<RayHit>(20);
            foreach (BVHNodeRayHit<T> hit in nodeHits)
            {
                if (objectFilter != null)
                {
                    if (!objectFilter.Filter(hit.HitNode.Data))
                    {
                        continue;
                    }
                }

                hit.HitNode.Data.SetPathToObjectCollider(pathDatas, this, hit.HitNode);
                hit.HitNode.Data.Raycast(ray, sortedObjectHits);
            }

            RayHit.SortByHitDistance(sortedObjectHits);

            return sortedObjectHits;
        }

        public void Clear() => Tree.Clear();

#if UNITY_EDITOR

        #region Gizmos

        public void DrawRaycast(Ray ray, Color nodeColor) => Tree.DrawRaycast(ray, Matrix4x4.identity, nodeColor);

        public void DrawAllCells(Color nodeColor) => Tree.DrawAllCells(Matrix4x4.identity, nodeColor);

        #endregion

#endif
    }
}
