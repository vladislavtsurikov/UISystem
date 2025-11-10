#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
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
                CustomEditorGUILayout.Toggle(new GUIContent("Draw Solid Disc"), _element.DrawSolidDisc);
            _element.CircleColor =
                CustomEditorGUILayout.ColorField(new GUIContent("Сircle Color"), _element.CircleColor);
            _element.CirclePixelWidth = CustomEditorGUILayout.Slider(new GUIContent("Сircle Pixel Width"),
                _element.CirclePixelWidth, 1f, 5f);
        }
    }
}
#endif
