using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.Math.Runtime
{
    public static class Matrix4x4Ex
    {
        public static Ray TransformRay(this Matrix4x4 mtx, Ray ray)
        {
            Vector3 origin = mtx.MultiplyPoint(ray.origin);
            Vector3 direction = mtx.MultiplyVector(ray.direction).normalized;

            return new Ray(origin, direction);
        }

        public static Matrix4x4 GetRelativeTransform(this Matrix4x4 matrix, Matrix4x4 referenceTransform) =>
            referenceTransform.inverse * matrix;

        public static Matrix4x4 RotationFromRightUp(Vector3 right, Vector3 up)
        {
            right.Normalize();
            up.Normalize();
            Vector3 look = Vector3.Cross(up, right).normalized;

            Matrix4x4 matrix = Matrix4x4.identity;

            matrix[0, 0] = right.x;
            matrix[1, 0] = right.y;
            matrix[2, 0] = right.z;

            matrix[0, 1] = up.x;
            matrix[1, 1] = up.y;
            matrix[2, 1] = up.z;

            matrix[0, 2] = look.x;
            matrix[1, 2] = look.y;
            matrix[2, 2] = look.z;

            return matrix;
        }


        public static Vector3 GetTranslation(this Matrix4x4 matrix) => matrix.GetColumn(3);

        public static Vector3 GetNormalizedAxis(this Matrix4x4 matrix, int axisIndex)
        {
            Vector3 axis = matrix.GetColumn(axisIndex);
            return Vector3.Normalize(axis);
        }

        public static Vector3[] GetNormalizedAxes(this Matrix4x4 matrix) =>
            new[] { matrix.GetNormalizedAxis(0), matrix.GetNormalizedAxis(1), matrix.GetNormalizedAxis(2) };

        public static List<Vector3> TransformPoints(this Matrix4x4 matrix, List<Vector3> points)
        {
            if (points.Count == 0)
            {
                return new List<Vector3>();
            }

            var transformedPts = new List<Vector3>(points.Count);
            foreach (Vector3 pt in points)
            {
                transformedPts.Add(matrix.MultiplyPoint(pt));
            }

            return transformedPts;
        }

        public static Vector3 TransformPoint(this Matrix4x4 matrix, Vector3 point) => matrix.MultiplyPoint(point);
    }
}
