#if UNITY_EDITOR
using System.Collections.Generic;
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
using VladislavTsurikov.MegaWorld.Runtime.TextureStamperTool;

namespace VladislavTsurikov.MegaWorld.Editor.TextureStamperTool
{
    public static class TextureStamperVisualisation
    {
        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.Selected)]
        private static void DrawGizmoForArea(TextureStamper textureStamper, GizmoType gizmoType)
        {
            var isFaded = (int)gizmoType == (int)GizmoType.NonSelected ||
                          (int)gizmoType == (int)GizmoType.NotInSelectionHierarchy || (int)gizmoType ==
                          (int)GizmoType.NonSelected + (int)GizmoType.NotInSelectionHierarchy;

            if (textureStamper.Area.DrawHandleIfNotSelected == false)
            {
                if (isFaded)
                {
                    return;
                }
            }

            var bounds = new Bounds(Camera.current.transform.position, new Vector3(50f, 50f, 50f));

            OverlapVisualisation.Draw(bounds, textureStamper.Data);

            var opacity = isFaded ? 0.5f : 1.0f;

            DrawStamperVisualisationIfNecessary(textureStamper, opacity);

            AreaVisualisation.DrawBox(textureStamper.Area, opacity);

            if (textureStamper.Area.UseSpawnCells)
            {
                DebugCells(textureStamper);
            }
        }

        private static void DrawStamperVisualisationIfNecessary(TextureStamper textureStamper, float multiplyAlpha)
        {
            if (textureStamper.StamperControllerSettings.Visualisation == false)
            {
                return;
            }

            if (textureStamper.Data.SelectedData.HasOneSelectedGroup() == false)
            {
                return;
            }

            if (!ToolUtility.IsToolSupportSelectedResourcesType(textureStamper.GetType(), textureStamper.Data))
            {
                return;
            }

            Group group = textureStamper.Data.SelectedData.SelectedGroup;

            LayerSettings layerSettings = GlobalCommonComponentSingleton<LayerSettings>.Instance;

            RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(textureStamper.Area.Bounds.center),
                layerSettings.GetCurrentPaintLayers(group.PrototypeType));
            if (rayHit != null)
            {
                BoxArea area = textureStamper.Area.GetAreaVariables(rayHit);
                textureStamper.StamperVisualisation.Draw(area, textureStamper.Data,
                    GlobalCommonComponentSingleton<LayerSettings>.Instance, multiplyAlpha);
            }
        }

        private static void DebugCells(TextureStamper textureStamper)
        {
            if (textureStamper.Area.ShowCells)
            {
                List<Bounds> cellList = textureStamper.Area.CellList;

                for (var i = 0; i <= cellList.Count - 1; i++)
                {
                    Gizmos.color = new Color(0, 1, 1, 1);
                    Gizmos.DrawWireCube(cellList[i].center, cellList[i].size);
                }
            }
        }
    }
}
#endif
