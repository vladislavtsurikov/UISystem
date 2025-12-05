using System;
using UnityEngine;
using VladislavTsurikov.Math.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings
{
    [Serializable]
    public abstract class OverlapShape
    {
        public abstract OBB GetOBB(Vector3 center, Vector3 scale, Quaternion rotation, Vector3 extents);
        public abstract bool Intersects(OverlapInstance instance, OverlapInstance spawnInstance);
#if UNITY_EDITOR
        public abstract void DrawOverlapVisualisation(Vector3 position, Vector3 scale, Quaternion rotation,
            Vector3 extents);
#endif
    }
}
