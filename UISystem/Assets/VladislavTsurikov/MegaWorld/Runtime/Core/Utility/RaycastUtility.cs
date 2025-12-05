using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.Utility
{
    public static class RaycastUtility
    {
        public static RayHit Raycast(Ray ray, LayerMask layersMask)
        {
            if (PreferenceElementSingleton<RaycastPreferenceSettings>.Instance.RaycastType == RaycastType.UnityRaycast)
            {
                return UnityRaycast(ray, layersMask);
            }
#if UNITY_EDITOR
            return ColliderUtility.Raycast(ray, layersMask);
#else
                return UnityRaycast(ray, layersMask);
#endif
        }

        private static RayHit UnityRaycast(Ray ray, LayerMask layersMask)
        {
            if (Physics.Raycast(ray, out RaycastHit hitInfo,
                    PreferenceElementSingleton<RaycastPreferenceSettings>.Instance.MaxRayDistance, layersMask))
            {
                var rayHit = new RayHit(hitInfo.transform.gameObject, hitInfo.normal, hitInfo.point, hitInfo.distance);
                return rayHit;
            }

            return null;
        }
    }
}
