#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Editor.Common.Stamper;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper;
using VladislavTsurikov.MegaWorld.Runtime.Core;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;

namespace VladislavTsurikov.MegaWorld.Editor.GravitySpawner
{
    public class GravitySpawnerVisualisation
    {
        public readonly StamperMaskFilterVisualisation StamperMaskFilterVisualisation = new();

        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.Selected)]
        private static void DrawGizmoForArea(Runtime.GravitySpawner.GravitySpawner stamper, GizmoType gizmoType)
        {
            var isFaded = (int)gizmoType == (int)GizmoType.NonSelected ||
                          (int)gizmoType == (int)GizmoType.NotInSelectionHierarchy || (int)gizmoType ==
                          (int)GizmoType.NonSelected + (int)GizmoType.NotInSelectionHierarchy;

            if (stamper.Area.DrawHandleIfNotSelected == false)
            {
                if (isFaded)
                {
                    return;
                }
            }

            var opacity = isFaded ? 0.5f : 1.0f;

            stamper.StamperVisualisation.DrawStamperVisualisationIfNecessary(stamper, opacity);

            AreaVisualisation.DrawBox(stamper.Area, opacity);
        }

        private void DrawStamperVisualisationIfNecessary(Runtime.GravitySpawner.GravitySpawner stamper,
            float multiplyAlpha)
        {
            if (stamper.StamperControllerSettings.Visualisation == false)
            {
                return;
            }

            if (stamper.Data.SelectedData.HasOneSelectedGroup() == false)
            {
                return;
            }

            if (!ToolUtility.IsToolSupportSelectedResourcesType(stamper.GetType(), stamper.Data))
            {
                return;
            }

            Group group = stamper.Data.SelectedData.SelectedGroup;

            LayerSettings layerSettings = GlobalCommonComponentSingleton<LayerSettings>.Instance;

            RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(stamper.Area.Bounds.center),
                layerSettings.GetCurrentPaintLayers(group.PrototypeType));

            if (rayHit != null)
            {
                BoxArea area = stamper.Area.GetAreaVariables(rayHit);
                Draw(area, stamper.Data, GlobalCommonComponentSingleton<LayerSettings>.Instance, multiplyAlpha);
            }
        }

        private void Draw(BoxArea area, SelectionData data, LayerSettings layerSettings, float multiplyAlpha)
        {
            if (area == null || area.RayHit == null)
            {
                return;
            }

            Group group = data.SelectedData.SelectedGroup;

            if (group.PrototypeType == typeof(PrototypeGameObject) ||
                group.PrototypeType == typeof(PrototypeTerrainObject))
            {
                var filterSettings = (FilterSettings)group.GetElement(typeof(Runtime.GravitySpawner.GravitySpawner),
                    typeof(FilterSettings));

                if (filterSettings.FilterType != FilterType.MaskFilter)
                {
                    SimpleFilterVisualisation.DrawSimpleFilter(group, area, filterSettings.SimpleFilter, layerSettings);
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
#endif
