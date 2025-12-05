using UnityEngine;
using VladislavTsurikov.Math.Runtime;

namespace VladislavTsurikov.BVH.Runtime
{
    public class BVHNodeAABB<TData> : BVHNode<TData>
        where TData : class
    {
        private Vector3 _max;
        private Vector3 _min;

        public BVHNodeAABB()
        {
        }

        public BVHNodeAABB(TData data) : base(data)
        {
        }

        public Vector3 Min
        {
            get => _min;
            set => _min = value;
        }

        public Vector3 Max
        {
            get => _max;
            set => _max = value;
        }

        public override Vector3 Position
        {
            get => (_min + _max) * 0.5f;
            set
            {
                Vector3 extents = (_max - _min) * 0.5f;
                _min = value - extents;
                _max = value + extents;
            }
        }

        public override Vector3 Size
        {
            get => _max - _min;
            set
            {
                Vector3 position = (_min + _max) * 0.5f;
                Vector3 extents = value * 0.5f;

                _min = position - extents;
                _max = position + extents;
            }
        }

        public override bool Raycast(Ray ray, out float t)
        {
            var bounds = new Bounds(Position, Size);
            return bounds.IntersectRay(ray, out t);
        }

        public override bool IntersectsBox(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation) =>
            BoxMath.BoxIntersectsBox(boxCenter, boxSize, boxRotation, Position, Size, Quaternion.identity);

        public override bool IntersectsSphere(Vector3 sphereCenter, float sphereRadius)
        {
            Vector3 closestPt = BoxMath.CalcBoxPtClosestToPt(sphereCenter, Position, Size, Quaternion.identity);
            return (closestPt - sphereCenter).sqrMagnitude <= sphereRadius * sphereRadius;
        }

        protected override void EncapsulateNode(BVHNode<TData> node)
        {
            var aabbNode = node as BVHNodeAABB<TData>;
            Vector3 aabbMin = aabbNode.Min;
            Vector3 aabbMax = aabbNode.Max;

            if (_min.x > aabbMin.x)
            {
                _min.x = aabbMin.x;
            }

            if (_min.y > aabbMin.y)
            {
                _min.y = aabbMin.y;
            }

            if (_min.z > aabbMin.z)
            {
                _min.z = aabbMin.z;
            }

            if (_max.x < aabbMax.x)
            {
                _max.x = aabbMax.x;
            }

            if (_max.y < aabbMax.y)
            {
                _max.y = aabbMax.y;
            }

            if (_max.z < aabbMax.z)
            {
                _max.z = aabbMax.z;
            }
        }
    }
}
