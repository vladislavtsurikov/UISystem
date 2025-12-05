using System.Collections.Generic;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility
{
    public static class UpdateFilterMask
    {
        public static void UpdateFilterMaskForPrototypes(IReadOnlyList<Prototype> prototypes, BoxArea boxArea)
        {
            if (boxArea.TerrainUnder == null)
            {
                return;
            }

            foreach (Prototype proto in prototypes)
            {
                var maskFilterComponentSettings =
                    (MaskFilterComponentSettings)proto.GetElement(typeof(MaskFilterComponentSettings));
                FilterMaskOperation.UpdateMaskTexture(maskFilterComponentSettings, boxArea);
            }
        }
    }
}
