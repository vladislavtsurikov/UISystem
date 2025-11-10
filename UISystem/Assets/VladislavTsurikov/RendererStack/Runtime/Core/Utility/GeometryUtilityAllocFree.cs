using System;
using System.Reflection;
using UnityEngine;

namespace VladislavTsurikov.RendererStack.Runtime.Core.Utility
{
    public static class GeometryUtilityAllocFree
    {
        public static readonly Plane[] FrustumPlanes = new Plane[6];

        private static readonly Action<Plane[], Matrix4x4> _internalExtractPlanes =
            (Action<Plane[], Matrix4x4>)Delegate.CreateDelegate(
                typeof(Action<Plane[], Matrix4x4>),
                // ReSharper disable once AssignNullToNotNullAttribute
                typeof(GeometryUtility).GetMethod("Internal_ExtractPlanes",
                    BindingFlags.Static | BindingFlags.NonPublic));

        public static void CalculateFrustumPlanes(Camera camera) =>
            _internalExtractPlanes(FrustumPlanes, camera.projectionMatrix * camera.worldToCameraMatrix);
    }
}
