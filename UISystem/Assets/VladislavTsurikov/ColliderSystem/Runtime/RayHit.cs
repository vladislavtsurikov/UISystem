using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.ColliderSystem.Runtime
{
    public class RayHit
    {
        private Vector3 _normal;

        public RayHit(object hitObject, Vector3 hitNormal, Vector3 point, float distance)
        {
            Object = hitObject;
            Point = point;
            Distance = distance;
            _normal = hitNormal;
            Plane = new Plane(_normal, Point);
        }

        public RayHit(object hitObject, MeshRayHit meshRayHit)
        {
            Object = hitObject;
            Point = meshRayHit.HitPoint;
            Distance = meshRayHit.HitEnter;
            _normal = meshRayHit.HitNormal;
            Plane = new Plane(_normal, Point);
            MeshRayHit = meshRayHit;
        }

        public object Object { get; }

        public Vector3 Point { get; }

        public float Distance { get; }

        public Vector3 Normal => _normal.normalized;
        public Plane Plane { get; }

        public MeshRayHit MeshRayHit { get; }

        public static void SortByHitDistance(List<RayHit> hits) => hits.Sort(delegate(RayHit h0, RayHit h1)
        {
            return h0.Distance.CompareTo(h1.Distance);
        });
    }
}
