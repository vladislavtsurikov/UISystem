using UnityEngine;

namespace VladislavTsurikov.BVH.Runtime
{
    public class BVHNodeRayHit<TNodeData> where TNodeData : class
    {
        public BVHNodeRayHit(Ray ray, BVHNode<TNodeData> hitNode, float distance)
        {
            HitNode = hitNode;
            Distance = distance;
            HitPoint = ray.GetPoint(Distance);
        }

        public BVHNode<TNodeData> HitNode { get; }

        public float Distance { get; }

        public Vector3 HitPoint { get; }
    }
}
