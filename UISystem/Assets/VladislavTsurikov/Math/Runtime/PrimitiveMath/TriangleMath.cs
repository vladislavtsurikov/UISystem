using UnityEngine;

namespace VladislavTsurikov.Math.Runtime
{
    public static class TriangleMath
    {
        public static bool Raycast(Ray ray, out float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            t = 0.0f;

            float rayEnter;
            var trianglePlane = new Plane(p0, p1, p2);
            if (trianglePlane.Raycast(ray, out rayEnter) &&
                Contains3DPoint(ray.GetPoint(rayEnter), false, p0, p1, p2))
            {
                t = rayEnter;
                return true;
            }

            return false;
        }

        public static bool Raycast(Ray ray, out float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 normal)
        {
            t = 0.0f;

            float rayEnter;
            var trianglePlane = new Plane(normal, p0);

            if (trianglePlane.Raycast(ray, out rayEnter) &&
                Contains3DPoint(ray.GetPoint(rayEnter), false, p0, p1, p2, normal))
            {
                t = rayEnter;
                return true;
            }

            return false;
        }

        public static bool RaycastWire(Ray ray, out float t, Vector3 p0, Vector3 p1, Vector3 p2, float wireEps)
        {
            t = 0.0f;

            float rayEnter;
            var trianglePlane = new Plane(p0, p1, p2);
            if (trianglePlane.Raycast(ray, out rayEnter))
            {
                Vector3 intersectPt = ray.GetPoint(rayEnter);
                var distToSegment = intersectPt.GetDistanceToSegment(p0, p1);
                if (distToSegment <= wireEps)
                {
                    t = rayEnter;
                    return true;
                }

                distToSegment = intersectPt.GetDistanceToSegment(p1, p2);
                if (distToSegment <= wireEps)
                {
                    t = rayEnter;
                    return true;
                }

                distToSegment = intersectPt.GetDistanceToSegment(p2, p0);
                if (distToSegment <= wireEps)
                {
                    t = rayEnter;
                    return true;
                }
            }

            return false;
        }

        public static bool Contains3DPoint(Vector3 point, bool checkOnPlane, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            Vector3 edge0 = p1 - p0;
            Vector3 edge1 = p2 - p1;
            Vector3 edge2 = p0 - p2;
            Vector3 normal = Vector3.Cross(edge0, -edge2).normalized;

            if (checkOnPlane)
            {
                var distanceToPt = Vector3.Dot(point - p0, normal);
                if (Mathf.Abs(distanceToPt) > 1e-5f)
                {
                    return false;
                }
            }

            Vector3 edgeNormal = Vector3.Cross(edge0, normal).normalized;
            if (Vector3.Dot(point - p0, edgeNormal) > 0.0f)
            {
                return false;
            }

            edgeNormal = Vector3.Cross(edge1, normal).normalized;
            if (Vector3.Dot(point - p1, edgeNormal) > 0.0f)
            {
                return false;
            }

            edgeNormal = Vector3.Cross(edge2, normal).normalized;
            if (Vector3.Dot(point - p2, edgeNormal) > 0.0f)
            {
                return false;
            }

            return true;
        }

        public static bool Contains3DPoint(Vector3 point, bool checkOnPlane, Vector3 p0, Vector3 p1, Vector3 p2,
            Vector3 normal)
        {
            Vector3 edge0 = p1 - p0;
            Vector3 edge1 = p2 - p1;
            Vector3 edge2 = p0 - p2;

            if (checkOnPlane)
            {
                var distanceToPt = Vector3.Dot(point - p0, normal);
                if (Mathf.Abs(distanceToPt) > 1e-5f)
                {
                    return false;
                }
            }

            Vector3 edgeNormal = Vector3.Cross(edge0, normal).normalized;
            if (Vector3.Dot(point - p0, edgeNormal) > 0.0f)
            {
                return false;
            }

            edgeNormal = Vector3.Cross(edge1, normal).normalized;
            if (Vector3.Dot(point - p1, edgeNormal) > 0.0f)
            {
                return false;
            }

            edgeNormal = Vector3.Cross(edge2, normal).normalized;
            if (Vector3.Dot(point - p2, edgeNormal) > 0.0f)
            {
                return false;
            }

            return true;
        }

        public static bool Contains2DPoint(Vector2 point, Vector2 p0, Vector2 p1, Vector2 p2) =>
            Contains3DPoint(point, false, p0, p1, p2);
    }
}
