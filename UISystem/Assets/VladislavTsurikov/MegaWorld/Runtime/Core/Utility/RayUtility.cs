using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.Utility
{
    public static class RayUtility
    {
        public static Ray GetRayDown(Vector3 point) =>
            new(
                new Vector3(point.x, point.y + PreferenceElementSingleton<RaycastPreferenceSettings>.Instance.Offset,
                    point.z), Vector3.down);

        public static Ray GetRayFromCameraPosition(Vector3 point)
        {
            Vector3 position = Camera.current.transform.position;
            return new Ray(position, (point - position).normalized);
        }
    }
}
