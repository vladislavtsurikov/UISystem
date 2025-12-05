#if UNITY_EDITOR
using VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Stamper
{
    public class StamperVisualisation
    {
        public StamperMaskFilterVisualisation StamperMaskFilterVisualisation = new();

        public void Draw(BoxArea area, SelectionData data, LayerSettings layerSettings, float multiplyAlpha)
        {
            if (area == null || area.RayHit == null)
            {
                return;
            }

            if (data.SelectedData.HasOneSelectedGroup())
            {
                Group group = data.SelectedData.SelectedGroup;

                if (group.PrototypeType == typeof(PrototypeGameObject) ||
                    group.PrototypeType == typeof(PrototypeTerrainObject))
                {
                    var filterSettings = (FilterSettings)group.GetElement(typeof(FilterSettings));

                    if (filterSettings.FilterType != FilterType.MaskFilter)
                    {
                        SimpleFilterVisualisation.DrawSimpleFilter(group, area, filterSettings.SimpleFilter,
                            layerSettings);
                    }
                    else
                    {
                        StamperMaskFilterVisualisation.DrawMaskFilterVisualization(
                            filterSettings.MaskFilterComponentSettings.MaskFilterStack, area, multiplyAlpha);
                    }
                }
                else
                {
                    if (data.SelectedData.HasOneSelectedPrototype())
                    {
                        StamperMaskFilterVisualisation.DrawMaskFilterVisualization(
                            MaskFilterUtility.GetMaskFilterFromSelectedPrototype(data), area, multiplyAlpha);
                    }
                    else
                    {
                        DrawShaderVisualisationUtility.DrawAreaPreview(area);
                    }
                }
            }
        }
    }
}
#endif
