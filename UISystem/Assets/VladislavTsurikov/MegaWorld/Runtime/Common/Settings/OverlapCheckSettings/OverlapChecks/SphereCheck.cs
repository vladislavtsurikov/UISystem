using UnityEngine;
using VladislavTsurikov.ColorUtility.Runtime;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility.Repaint;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings.OverlapChecks
{
    public class SphereCheck : OverlapShape
    {
        public int Priority;
        public float Size = 3.5f;
        public float TrunkSize = 0.8f;
        public bool VegetationMode = true;
        public float ViabilitySize = 4f;

        public override OBB GetOBB(Vector3 center, Vector3 scale, Quaternion rotation, Vector3 extents)
        {
            Vector3 boundsSize;

            if (VegetationMode)
            {
                boundsSize = new Vector3(ViabilitySize * scale.x, ViabilitySize * scale.x, ViabilitySize * scale.x);
            }
            else
            {
                boundsSize = new Vector3(Size * scale.x, Size * scale.x, Size * scale.x);
            }

            var bounds = new Bounds { center = center, size = boundsSize };

            return new OBB(bounds, Quaternion.identity);
        }

        public override bool Intersects(OverlapInstance instance, OverlapInstance spawnInstance)
        {
            var viabilitySphere = new Sphere(instance.Position,
                instance.Obb.Size.x / 2);

            var otherViabilitySphere = new Sphere(spawnInstance.Position,
                spawnInstance.Obb.Size.x / 2);

            var supportVegetationMode = instance.OverlapCheckSettings.SphereCheck.VegetationMode;

            if (!spawnInstance.OverlapCheckSettings.SphereCheck.VegetationMode)
            {
                supportVegetationMode = false;
            }

            if (supportVegetationMode)
            {
                if (spawnInstance.OverlapCheckSettings.SphereCheck.Priority >
                    instance.OverlapCheckSettings.SphereCheck.Priority)
                {
                    var trunkRadius = instance.OverlapCheckSettings.SphereCheck.TrunkSize / 2 *
                                      (instance.Scale.x / 2);

                    var otherTrunkRadius = spawnInstance.OverlapCheckSettings.SphereCheck.TrunkSize / 2 *
                                           (spawnInstance.Scale.x / 2);

                    var trunkSphere = new Sphere(instance.Position, trunkRadius);
                    var otherTrunkSphere = new Sphere(spawnInstance.Position, otherTrunkRadius);

                    return trunkSphere.Intersects(otherTrunkSphere);
                }

                return viabilitySphere.Intersects(otherViabilitySphere);
            }

            return viabilitySphere.Intersects(otherViabilitySphere);
        }

#if UNITY_EDITOR
        public override void DrawOverlapVisualisation(Vector3 position, Vector3 scale, Quaternion rotation,
            Vector3 extents)
        {
            OBB obb = GetOBB(position, scale, rotation, extents);

            if (VegetationMode)
            {
                var trunkRadius = TrunkSize * scale.x / 2;
                var viabilityRadius = obb.Size.x / 2;

                Handles.color = Color.red;
                DrawHandles.CircleCap(1, position, Quaternion.LookRotation(Vector3.up), trunkRadius);

                Handles.color = Color.red.WithAlpha(0.1f);
                Handles.DrawSolidDisc(position, Vector3.up, trunkRadius);

                Handles.color = Color.blue;
                DrawHandles.CircleCap(1, position, Quaternion.LookRotation(Vector3.up), viabilityRadius);

                Handles.color = Color.blue.WithAlpha(0.1f);
                Handles.DrawSolidDisc(position, Vector3.up, viabilityRadius);
            }
            else
            {
                var radius = obb.Size.x / 2;

                Handles.color = Color.red;
                DrawHandles.CircleCap(1, position, Quaternion.LookRotation(Vector3.up), radius);

                Handles.color = Color.red.WithAlpha(0.1f);
                Handles.DrawSolidDisc(position, Vector3.up, radius);
            }
        }
#endif
    }
}
