#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ColorUtility.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.BrushSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility.Repaint;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;
using DrawHandles = VladislavTsurikov.UnityUtility.Editor.DrawHandles;

namespace VladislavTsurikov.MegaWorld.Editor.BrushPhysicsTool
{
    public static class BrushPhysicsToolVisualisation
    {
        public static void Draw(BoxArea area)
        {
            if (area == null || area.RayHit == null)
            {
                return;
            }

            var brushPhysicsToolSettings =
                (BrushPhysicsToolSettings)ToolsComponentStack.GetElement(typeof(BrushPhysicsTool),
                    typeof(BrushPhysicsToolSettings));

            VisualisationBrushHandlesPreference visualisationBrushHandlesPreference =
                PreferenceElementSingleton<VisualisationBrushHandlesPreference>.Instance;

            VisualisationUtility.DrawCircleHandles(area.BoxSize, area.RayHit);

            Color color = visualisationBrushHandlesPreference.CircleColor.WithAlpha(0.8f);
            Vector3 position = area.RayHit.Point + new Vector3(0, brushPhysicsToolSettings.PositionOffsetY, 0);
            DrawHandles.HandleButton(0, position, color, 0.7f);
        }
    }
}
#endif
