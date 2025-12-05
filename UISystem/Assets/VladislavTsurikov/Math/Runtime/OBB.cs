using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.Math.Runtime
{
    public struct OBB
    {
        private Vector3 _size;

        public bool IsValid { get; }

        public Vector3 Center { get; set; }

        public Vector3 Size
        {
            get => _size;
            set => _size = value.Abs();
        }

        public Vector3 Extents => Size * 0.5f;
        public Quaternion Rotation { get; set; }

        public Matrix4x4 RotationMatrix => Matrix4x4.TRS(Vector3.zero, Rotation, Vector3.one);
        public Vector3 Right => Rotation * Vector3.right;
        public Vector3 Up => Rotation * Vector3.up;
        public Vector3 Look => Rotation * Vector3.forward;

        public OBB(Vector3 center, Vector3 size)
        {
            Center = center;
            _size = size.Abs();
            Rotation = Quaternion.identity;
            IsValid = true;
        }

        public OBB(Vector3 center, Vector3 size, Quaternion rotation)
        {
            Center = center;
            _size = size.Abs();
            Rotation = rotation;
            IsValid = true;
        }

        public OBB(Vector3 center, Quaternion rotation)
        {
            Center = center;
            _size = Vector3.zero;
            Rotation = rotation;
            IsValid = true;
        }

        public OBB(Quaternion rotation)
        {
            Center = Vector3.zero;
            _size = Vector3.zero;
            Rotation = rotation;
            IsValid = true;
        }

        public OBB(Bounds bounds, Quaternion rotation)
        {
            Center = bounds.center;
            _size = bounds.size.Abs();
            Rotation = rotation;
            IsValid = true;
        }

        public OBB(AABB aabb)
        {
            Center = aabb.Center;
            _size = aabb.Size;
            Rotation = Quaternion.identity;
            IsValid = true;
        }

        public OBB(AABB aabb, Quaternion rotation)
        {
            Center = aabb.Center;
            _size = aabb.Size;
            Rotation = rotation;
            IsValid = true;
        }

        public OBB(AABB modelSpaceAABB, Transform worldTransform)
        {
            _size = Vector3.Scale(modelSpaceAABB.Size, worldTransform.lossyScale).Abs();
            Center = worldTransform.TransformPoint(modelSpaceAABB.Center);
            Rotation = worldTransform.rotation;
            IsValid = true;
        }

        public OBB(AABB modelSpaceAABB, Matrix4x4 worldTransform)
        {
            _size = Vector3.Scale(modelSpaceAABB.Size, worldTransform.lossyScale).Abs();

            Center = worldTransform.TransformPoint(modelSpaceAABB.Center);
            Rotation = worldTransform.rotation;
            IsValid = true;
        }

        public OBB(OBB copy)
        {
            _size = copy._size;
            Center = copy.Center;
            Rotation = copy.Rotation;
            IsValid = copy.IsValid;
        }

        public static OBB GetInvalid() => new();

        public void Inflate(float amount) => Size += Vector3Ex.FromValue(amount);

        public Matrix4x4 GetUnitBoxTransform()
        {
            if (!IsValid)
            {
                return Matrix4x4.identity;
            }

            return Matrix4x4.TRS(Center, Rotation, Size);
        }

        public List<Vector3> GetCornerPoints() => BoxMath.CalcBoxCornerPoints(Center, _size, Rotation);

        public List<Vector3> GetCenterAndCornerPoints()
        {
            List<Vector3> centerAndCorners = GetCornerPoints();
            centerAndCorners.Add(Center);

            return centerAndCorners;
        }

        public void Encapsulate(OBB otherOBB)
        {
            List<Vector3> otherPts = BoxMath.CalcBoxCornerPoints(otherOBB.Center, otherOBB.Size, otherOBB.Rotation);

            var transformMtx = Matrix4x4.TRS(Center, Rotation, Vector3.one);
            List<Vector3> modelPts = transformMtx.inverse.TransformPoints(otherPts);

            var modelAABB = new AABB(Vector3.zero, Size);
            modelAABB.Encapsulate(modelPts);

            Center = Rotation * modelAABB.Center + Center;
            Size = modelAABB.Size;
        }

        public bool IntersectsSphere(Vector3 sphereCenter, float sphereRadius)
        {
            var radiusSqr = sphereRadius * sphereRadius;

            Vector3 closestPt = BoxMath.CalcBoxPtClosestToPt(sphereCenter, Center, Size, Rotation);
            if ((closestPt - sphereCenter).sqrMagnitude <= radiusSqr)
            {
                return true;
            }

            return false;
        }

        public bool IntersectsOBB(OBB otherOBB) => BoxMath.BoxIntersectsBox(Center, _size, Rotation, otherOBB.Center,
            otherOBB.Size, otherOBB.Rotation);

        public Vector3 GetClosestPoint(Vector3 point) => BoxMath.CalcBoxPtClosestToPt(point, Center, _size, Rotation);
    }
}
