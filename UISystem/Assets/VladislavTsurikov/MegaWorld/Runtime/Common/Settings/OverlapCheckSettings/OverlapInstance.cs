using UnityEngine;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings
{
    public class OverlapInstance
    {
        public readonly Vector3 Extents;

        public readonly OverlapCheckSettings OverlapCheckSettings;

        public readonly Vector3 Position;
        public readonly Quaternion Rotation;
        public readonly Vector3 Scale;
        private OBB _obb;

        public OverlapInstance(OverlapCheckSettings overlapCheckSettings, Vector3 extents, Instance instance)
            : this(overlapCheckSettings, instance.Position, instance.Scale, instance.Rotation, extents)
        {
        }

        public OverlapInstance(OverlapCheckSettings overlapCheckSettings, Vector3 position, Vector3 scale,
            Quaternion rotation, Vector3 extents)
        {
            OverlapCheckSettings = overlapCheckSettings;

            Position = position;
            Scale = scale;
            Rotation = rotation;
            Extents = extents;
        }

        public OBB Obb
        {
            get
            {
                if (_obb.IsValid)
                {
                    return _obb;
                }

                _obb = OverlapCheckSettings.CurrentOverlapShape.GetOBB(Position, Scale, Rotation, Extents);

                return _obb;
            }
        }

        public bool Intersects(OverlapInstance spawnInstance)
        {
            if (OverlapCheckSettings.CurrentOverlapShape == spawnInstance.OverlapCheckSettings.CurrentOverlapShape)
            {
                return OverlapCheckSettings.CurrentOverlapShape.Intersects(this, spawnInstance);
            }

            OBB obb = Obb;
            return obb.IntersectsOBB(spawnInstance.Obb);
        }
    }
}
