#if UNITY_EDITOR
#if UNITY_2021_2_OR_NEWER
#else
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine.Experimental.TerrainAPI;
#endif
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.BrushSettings;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Utility.Repaint
{
    public static class VisualisationUtility
    {
        public static void DrawCircleHandles(float size, RayHit hit)
        {
            var radius = size / 2;
            if (PreferenceElementSingleton<VisualisationBrushHandlesPreference>.Instance.DrawSolidDisc)
            {
                Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.1f);
                Handles.DrawSolidDisc(hit.Point, hit.Normal, radius);
            }

            DrawCircle(size, hit);
        }

        private static void DrawCircle(float size, RayHit hit)
        {
            var localTransform = Matrix4x4.TRS(hit.Point, Quaternion.LookRotation(hit.Normal),
                new Vector3(size, size, size));

            Color color = PreferenceElementSingleton<VisualisationBrushHandlesPreference>.Instance.CircleColor;
            var thickness = PreferenceElementSingleton<VisualisationBrushHandlesPreference>.Instance.CirclePixelWidth;

            UnityUtility.Editor.DrawHandles.DrawCircleWithoutZTest(localTransform,
                Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(90, Vector3.right), Vector3.one), color, thickness);
        }
    }
}
#endif
