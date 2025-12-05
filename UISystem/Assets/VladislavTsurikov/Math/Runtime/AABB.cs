using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.Math.Runtime
{
    public struct AABB
    {
        private Vector3 _center;
        private Vector3 _size;

        public Vector3 Center
        {
            get => _center;
            set => _center = value;
        }

        public Vector3 Size
        {
            get => _size;
            set => _size = value.Abs();
        }

        public Vector3 Extents => _size * 0.5f;

        public Vector3 Min
        {
            get => _center - Extents;
            set => CalcCenterAndSize(Vector3.Min(value, Max), Max);
        }

        public Vector3 Max
        {
            get => _center + Extents;
            set => CalcCenterAndSize(Min, Vector3.Max(value, Min));
        }

        public bool IsValid => _size != Vector3.zero;

        public static AABB GetInvalid() => new();

        public AABB(Vector3 center, Vector3 size)
        {
            _center = center;
            _size = size.Abs();
        }

        public AABB(Bounds bounds)
        {
            _center = bounds.center;
            _size = bounds.size.Abs();
        }

        public AABB(IEnumerable<Vector3> pointCloud)
        {
            var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            foreach (Vector3 pt in pointCloud)
            {
                min = Vector3.Min(pt, min);
                max = Vector3.Max(pt, max);
            }

            _center = (min + max) * 0.5f;
            _size = max - min;
        }

        public AABB(IEnumerable<Vector2> pointCloud)
        {
            // Find the minimum and maximum extents of the point cloud
            var min = new Vector2(float.MaxValue, float.MaxValue);
            var max = new Vector2(float.MinValue, float.MinValue);
            foreach (Vector2 pt in pointCloud)
            {
                min = Vector2.Min(pt, min);
                max = Vector2.Max(pt, max);
            }

            // Calculate the center and size
            _center = (min + max) * 0.5f;
            _size = max - min;
        }

        public void Encapsulate(Vector3 point)
        {
            Vector3 min = Min;
            Vector3 max = Max;

            if (point.x < min.x)
            {
                min.x = point.x;
            }

            if (point.x > max.x)
            {
                max.x = point.x;
            }

            if (point.y < min.y)
            {
                min.y = point.y;
            }

            if (point.y > max.y)
            {
                max.y = point.y;
            }

            if (point.z < min.z)
            {
                min.z = point.z;
            }

            if (point.z > max.z)
            {
                max.z = point.z;
            }

            Min = min;
            Max = max;
        }

        public void Encapsulate(IEnumerable<Vector3> points)
        {
            foreach (Vector3 pt in points)
            {
                Encapsulate(pt);
            }
        }

        public void Encapsulate(AABB aabb)
        {
            Vector3 thisMin = Min;
            Vector3 thisMax = Max;

            Vector3 otherMin = aabb.Min;
            Vector3 otherMax = aabb.Max;

            if (otherMin.x < thisMin.x)
            {
                thisMin.x = otherMin.x;
            }

            if (otherMin.y < thisMin.y)
            {
                thisMin.y = otherMin.y;
            }

            if (otherMin.z < thisMin.z)
            {
                thisMin.z = otherMin.z;
            }

            if (otherMax.x > thisMax.x)
            {
                thisMax.x = otherMax.x;
            }

            if (otherMax.y > thisMax.y)
            {
                thisMax.y = otherMax.y;
            }

            if (otherMax.z > thisMax.z)
            {
                thisMax.z = otherMax.z;
            }

            Min = thisMin;
            Max = thisMax;
        }

        public void Inflate(float amount) => Size += Vector3Ex.FromValue(amount);

        public void Inflate(Vector3 amount) => Size += amount;

        public void Transform(Matrix4x4 transformMatrix) =>
            BoxMath.TransformBox(_center, _size, transformMatrix, out _center, out _size);

        public bool ContainsPoint(Vector3 point) => BoxMath.ContainsPoint(point, _center, _size, Quaternion.identity);

        public List<Vector3> GetCornerPoints() => BoxMath.CalcBoxCornerPoints(_center, _size, Quaternion.identity);

        public List<Vector3> GetCenterAndCornerPoints()
        {
            List<Vector3> centerAndCorners = GetCornerPoints();
            centerAndCorners.Add(Center);

            return centerAndCorners;
        }

        public List<Vector2> GetScreenCornerPoints(Camera camera)
        {
            List<Vector3> cornerPoints = GetCornerPoints();
            var screenCornerPoints = new List<Vector2>(cornerPoints.Count);

            foreach (Vector3 pt in cornerPoints)
            {
                screenCornerPoints.Add(camera.WorldToScreenPoint(pt));
            }

            return screenCornerPoints;
        }

        public List<Vector2> GetScreenCenterAndCornerPoints(Camera camera)
        {
            List<Vector3> allPoints = GetCenterAndCornerPoints();
            var screenPoints = new List<Vector2>(allPoints.Count);

            foreach (Vector3 pt in allPoints)
            {
                screenPoints.Add(camera.WorldToScreenPoint(pt));
            }

            return screenPoints;
        }

        public Rect GetScreenRectangle(Camera camera)
        {
            List<Vector2> screenCornerPoints = GetScreenCornerPoints(camera);

            Vector3 minScreenPoint = screenCornerPoints[0], maxScreenPoint = screenCornerPoints[0];
            for (var screenPointIndex = 1; screenPointIndex < screenCornerPoints.Count; ++screenPointIndex)
            {
                minScreenPoint = Vector3.Min(minScreenPoint, screenCornerPoints[screenPointIndex]);
                maxScreenPoint = Vector3.Max(maxScreenPoint, screenCornerPoints[screenPointIndex]);
            }

            return Rect.MinMaxRect(minScreenPoint.x, minScreenPoint.y, maxScreenPoint.x, maxScreenPoint.y);
        }

        private void CalcCenterAndSize(Vector3 min, Vector3 max)
        {
            _center = (min + max) * 0.5f;
            _size = max - min;
        }

        public bool IntersectsSphere(Vector3 sphereCenter, float sphereRadius)
        {
            var radiusSqr = sphereRadius * sphereRadius;

            Vector3 closestPt = BoxMath.CalcBoxPtClosestToPt(sphereCenter, Center, Size, Quaternion.identity);
            if ((closestPt - sphereCenter).sqrMagnitude <= radiusSqr)
            {
                return true;
            }

            return false;
        }

        public Bounds ToBounds() => new(_center, _size);
    }
}
