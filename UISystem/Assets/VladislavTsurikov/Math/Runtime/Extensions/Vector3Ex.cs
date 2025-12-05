using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.Math.Runtime
{
    public enum FromDirection
    {
        SurfaceNormal,
        X,
        Y,
        Z
    }

    public static class Vector3Ex
    {
        public static bool IsSameVector(this Vector3 a, Vector3 b, float epsilon = 0.001f) =>
            Mathf.Abs(a.x - b.x) < epsilon && Mathf.Abs(a.y - b.y) < epsilon && Mathf.Abs(a.z - b.z) < epsilon;

        public static void OffsetPoints(List<Vector3> points, Vector3 offset)
        {
            for (var ptIndex = 0; ptIndex < points.Count; ++ptIndex)
            {
                points[ptIndex] += offset;
            }
        }

        public static Vector2 ConvertDirTo2D(Vector3 start, Vector3 end, Camera camera)
        {
            Vector2 start2D = camera.WorldToScreenPoint(start);
            Vector2 end2D = camera.WorldToScreenPoint(end);
            return end2D - start2D;
        }

        public static Vector3 Abs(this Vector3 v) => new(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));

        public static Vector3 GetSignVector(this Vector3 v) => new(Mathf.Sign(v.x), Mathf.Sign(v.y), Mathf.Sign(v.z));

        public static float GetMaxAbsComp(this Vector3 v)
        {
            var maxAbsComp = Mathf.Abs(v.x);

            var absY = Mathf.Abs(v.y);
            if (absY > maxAbsComp)
            {
                maxAbsComp = absY;
            }

            var absZ = Mathf.Abs(v.z);
            if (absZ > maxAbsComp)
            {
                maxAbsComp = absZ;
            }

            return maxAbsComp;
        }

        public static float Dot(this Vector3 v1, Vector3 v2) => Vector3.Dot(v1, v2);

        public static float AbsDot(this Vector3 v1, Vector3 v2) => Mathf.Abs(Vector3.Dot(v1, v2));

        public static Vector3 FromValue(float value) => new(value, value, value);

        public static float GetDistanceToSegment(this Vector3 point, Vector3 point0, Vector3 point1)
        {
            Vector3 segmentDir = point1 - point0;
            var segmentLength = segmentDir.magnitude;
            segmentDir.Normalize();

            Vector3 fromStartToPt = point - point0;
            var projection = Vector3.Dot(segmentDir, fromStartToPt);

            if (projection >= 0.0f && projection <= segmentLength)
            {
                return (point0 + segmentDir * projection - point).magnitude;
            }

            if (projection < 0.0f)
            {
                return fromStartToPt.magnitude;
            }

            return (point1 - point).magnitude;
        }

        public static Vector3 ProjectOnSegment(this Vector3 point, Vector3 point0, Vector3 point1)
        {
            Vector3 segmentDir = (point1 - point0).normalized;
            return point0 + segmentDir * Vector3.Dot(point - point0, segmentDir);
        }

        public static int GetPointClosestToPoint(List<Vector3> points, Vector3 pt)
        {
            var minDistSq = float.MaxValue;
            var closestPtIndex = -1;

            for (var ptIndex = 0; ptIndex < points.Count; ++ptIndex)
            {
                Vector3 point = points[ptIndex];

                var distSq = (point - pt).sqrMagnitude;
                if (distSq < minDistSq)
                {
                    minDistSq = distSq;
                    closestPtIndex = ptIndex;
                }
            }

            return closestPtIndex;
        }

        public static Vector3 GetPointCloudCenter(IEnumerable<Vector3> ptCloud)
        {
            Vector3 max = FromValue(float.MinValue);
            Vector3 min = FromValue(float.MaxValue);

            foreach (Vector3 pt in ptCloud)
            {
                max = Vector3.Max(max, pt);
                min = Vector3.Min(min, pt);
            }

            return (max + min) * 0.5f;
        }

        public static Vector3 GetInverse(this Vector3 vector) => new(1.0f / vector.x, 1.0f / vector.y, 1.0f / vector.z);

        public static bool IsAligned(this Vector3 vector, Vector3 other, bool checkSameDirection)
        {
            if (!checkSameDirection)
            {
                var absDot = vector.AbsDot(other);
                return Mathf.Abs(absDot - 1.0f) < 1e-5f;
            }

            var dot = vector.Dot(other);
            return dot > 0.0f && Mathf.Abs(dot - 1.0f) < 1e-5f;
        }

        public static bool IsBigger(this Vector3 vector, Vector3 other)
        {
            if (vector.x > other.x
                || vector.y > other.y
                || vector.z > other.z)
            {
                return true;
            }

            return false;
        }

        public static int GetMostAligned(Vector3[] vectors, Vector3 dir, bool checkSameDirection)
        {
            if (vectors.Length == 0)
            {
                return -1;
            }

            var bestAlignment = float.MinValue;
            var bestIndex = -1;

            if (!checkSameDirection)
            {
                // Loop through each test vector
                for (var dirIndex = 0; dirIndex < vectors.Length; ++dirIndex)
                {
                    // Calculate the absolute dot product with 'dir'. If this is gerater
                    // than what we have so far, it means we found vector which is more
                    // aligned with 'dir'.
                    Vector3 testDir = vectors[dirIndex];
                    var absDot = testDir.AbsDot(dir);
                    if (absDot > bestAlignment)
                    {
                        bestAlignment = absDot;
                        bestIndex = dirIndex;
                    }
                }

                return bestIndex;
            }

            // Loop through each test vector
            for (var dirIndex = 0; dirIndex < vectors.Length; ++dirIndex)
            {
                // Calculate the dot product with 'dir'. If this is gerater than 0
                // and greater than what we have so far, it means we found vector 
                // which is more aligned with 'dir'.
                Vector3 testDir = vectors[dirIndex];
                var dot = testDir.Dot(dir);
                if (dot > 0.0f && dot > bestAlignment)
                {
                    bestAlignment = dot;
                    bestIndex = dirIndex;
                }
            }

            return bestIndex;
        }

        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation) =>
            rotation * (point - pivot) + pivot;

        public static void GetOrientation(Vector3 normal, FromDirection mode, float weightToNormal, out Vector3 upwards,
            out Vector3 right, out Vector3 forward)
        {
            switch (mode)
            {
                case FromDirection.SurfaceNormal:
                    upwards = Vector3.Lerp(Vector3.up, normal, weightToNormal);
                    break;
                case FromDirection.X:
                    upwards = new Vector3(1, 0, 0);
                    break;
                default:
                case FromDirection.Y:
                    upwards = new Vector3(0, 1, 0);
                    break;
                case FromDirection.Z:
                    upwards = new Vector3(0, 0, 1);
                    break;
            }

            TransformAxes.GetRightForward(upwards, out right, out forward);
        }
    }
}
