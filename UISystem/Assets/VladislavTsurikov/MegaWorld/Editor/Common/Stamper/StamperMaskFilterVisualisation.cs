#if UNITY_EDITOR
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Stamper
{
    public class StamperMaskFilterVisualisation : MaskFilterVisualisation
    {
        protected override bool IsNeedUpdateMask(MaskFilterStack maskFilterStack, BoxArea boxArea)
        {
            if (NeedUpdateMask)
            {
                return NeedUpdateMask;
            }

            if (_filterContext == null)
            {
                NeedUpdateMask = true;

                return NeedUpdateMask;
            }

            if (PastMaskFilterStack != maskFilterStack || NeedUpdateMask)
            {
                NeedUpdateMask = true;

                return NeedUpdateMask;
            }

            return false;
        }
    }
}
#endif
