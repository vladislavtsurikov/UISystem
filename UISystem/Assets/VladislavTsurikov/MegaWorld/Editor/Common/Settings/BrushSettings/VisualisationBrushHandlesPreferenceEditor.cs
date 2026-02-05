#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Nody.Editor.Core;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.BrushSettings;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.BrushSettings
{
    [ElementEditor(typeof(VisualisationBrushHandlesPreference))]
    public class VisualisationBrushHandlesPreferenceEditor : IMGUIElementEditor
    {
        private VisualisationBrushHandlesPreference _element => (VisualisationBrushHandlesPreference)Target;

        public override void OnGUI()
        {
            _element.DrawSolidDisc =
                EditorGUILayout.Toggle(new GUIContent("Draw Solid Disc"), _element.DrawSolidDisc);
            _element.CircleColor =
                EditorGUILayout.ColorField(new GUIContent("Сircle Color"), _element.CircleColor);
            _element.CirclePixelWidth = EditorGUILayout.Slider(new GUIContent("Сircle Pixel Width"),
                _element.CirclePixelWidth, 1f, 5f);
        }
    }
}
#endif
