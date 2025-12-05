#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Editor.Common.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper;
using VladislavTsurikov.MegaWorld.Runtime.Core;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;

namespace VladislavTsurikov.MegaWorld.Editor.TerrainSpawner
{
    public static class TerrainSpawnerVisualisation
    {
        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.Selected)]
        private static void DrawGizmoForArea(Runtime.TerrainSpawner.TerrainSpawner stamper, GizmoType gizmoType)
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

            var bounds = new Bounds(Camera.current.transform.position, new Vector3(50f, 50f, 50f));

            OverlapVisualisation.Draw(bounds, stamper.Data);

            var opacity = isFaded ? 0.5f : 1.0f;

            DrawStamperVisualisationIfNecessary(stamper, opacity);

            AreaVisualisation.DrawBox(stamper.Area, opacity);
        }

        private static void DrawStamperVisualisationIfNecessary(Runtime.TerrainSpawner.TerrainSpawner stamper,
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
                stamper.StamperVisualisation.Draw(area, stamper.Data,
                    GlobalCommonComponentSingleton<LayerSettings>.Instance, multiplyAlpha);
            }
        }
    }
}
#endif
