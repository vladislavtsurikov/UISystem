using UnityEngine;
using VladislavTsurikov.ColorUtility.Runtime;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings.OverlapChecks
{
    public enum BoundsCheckType
    {
        Custom,
        BoundsPrefab
    }

    public class OBBCheck : OverlapShape
    {
        public Vector3 BoundsSize = Vector3.one;
        public BoundsCheckType BoundsType = BoundsCheckType.BoundsPrefab;
        public float MultiplyBoundsSize = 1;
        public bool UniformBoundsSize;

        public override OBB GetOBB(Vector3 center, Vector3 scale, Quaternion rotation, Vector3 extents)
        {
            Vector3 boundsSize = Vector3.zero;

            if (BoundsType == BoundsCheckType.Custom)
            {
                if (UniformBoundsSize)
                {
                    boundsSize.x = BoundsSize.x;
                    boundsSize.y = BoundsSize.x;
                    boundsSize.z = BoundsSize.x;
                }
                else
                {
                    boundsSize = BoundsSize;
                }
            }
            else if (BoundsType == BoundsCheckType.BoundsPrefab)
            {
                boundsSize.x = scale.x * (extents.x * 2);
                boundsSize.y = scale.y * (extents.y * 2);
                boundsSize.z = scale.z * (extents.z * 2);
            }

            boundsSize.x *= MultiplyBoundsSize;
            boundsSize.y *= MultiplyBoundsSize;
            boundsSize.z *= MultiplyBoundsSize;

            var bounds = new Bounds { center = center, size = boundsSize };

            return new OBB(bounds, rotation);
        }

        public override bool Intersects(OverlapInstance instance, OverlapInstance spawnInstance)
        {
            OBB obb = instance.Obb;
            OBB otherObb = spawnInstance.Obb;

            return obb.IntersectsOBB(otherObb);
        }

#if UNITY_EDITOR
        public override void DrawOverlapVisualisation(Vector3 position, Vector3 scale, Quaternion rotation,
            Vector3 extents)
        {
            Color color = Color.green.WithAlpha(0.5f);

            OBB obb = GetOBB(position, scale, rotation, extents);

            GizmosEx.PushColor(color);
            GizmosEx.PushMatrix(Matrix4x4.TRS(obb.Center, obb.Rotation, obb.Size));
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
            GizmosEx.PopMatrix();
            GizmosEx.PopColor();
        }
#endif
    }
}
