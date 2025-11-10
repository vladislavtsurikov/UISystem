#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings
{
    [ElementEditor(typeof(VisualisationSimpleFilterPreference))]
    public class VisualisationSimpleFilterPreferenceEditor : IMGUIElementEditor
    {
        private VisualisationSimpleFilterPreference _element => (VisualisationSimpleFilterPreference)Target;

        public override void OnGUI()
        {
            _element.EnableSpawnVisualization = CustomEditorGUILayout.Toggle(
                new GUIContent("Enable",
                    "I recommend turning off visualization if rendering slows down performance while spawning"),
                _element.EnableSpawnVisualization);

            if (_element.EnableSpawnVisualization)
            {
                _element.VisualiserResolution = CustomEditorGUILayout.IntSlider(new GUIContent("Visualiser Resolution"),
                    _element.VisualiserResolution, 1, 60);

                _element.HandlesType =
                    (HandlesType)CustomEditorGUILayout.EnumPopup(new GUIContent("Handles Type"), _element.HandlesType);
                _element.HandleResizingType =
                    (HandleResizingType)CustomEditorGUILayout.EnumPopup(new GUIContent("Handle Resizing Type"),
                        _element.HandleResizingType);

                if (_element.HandleResizingType == HandleResizingType.CustomSize)
                {
                    EditorGUI.indentLevel++;

                    _element.CustomHandleSize = CustomEditorGUILayout.Slider(new GUIContent("Handle Size"),
                        _element.CustomHandleSize, 0.1f, 3f);

                    EditorGUI.indentLevel--;
                }

                _element.ColorHandlesType =
                    (ColorHandlesType)CustomEditorGUILayout.EnumPopup(new GUIContent("Color Handles Type"),
                        _element.ColorHandlesType);

                if (_element.ColorHandlesType == ColorHandlesType.Custom)
                {
                    EditorGUI.indentLevel++;

                    _element.ActiveColor =
                        CustomEditorGUILayout.ColorField(new GUIContent("Active Color"), _element.ActiveColor);
                    _element.InactiveColor =
                        CustomEditorGUILayout.ColorField(new GUIContent("Inactive Color"), _element.InactiveColor);

                    EditorGUI.indentLevel--;
                }

                _element.Alpha = CustomEditorGUILayout.Slider(new GUIContent("Alpha"), _element.Alpha, 0, 1);
            }
        }
    }
}
#endif
