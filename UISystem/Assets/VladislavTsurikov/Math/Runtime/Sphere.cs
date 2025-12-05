using UnityEngine;

namespace VladislavTsurikov.Math.Runtime
{
    public struct Sphere
    {
        public bool IsValid { get; }

        public Vector3 Center;
        public float Radius;

        public Sphere(Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
            IsValid = true;
        }

        public bool Contains(Vector3 point) => Vector3.Distance(point, Center) <= Radius;

        public bool Intersects(Sphere otherSphere)
        {
            var distance = Vector3.Distance(Center, otherSphere.Center);

            if (distance < Radius + otherSphere.Radius)
            {
                return true;
            }

            return false;
        }

        public bool Intersects(OBB obb) => obb.IntersectsSphere(Center, Radius);

        public bool Intersects(AABB aabb) => aabb.IntersectsSphere(Center, Radius);
    }
}
