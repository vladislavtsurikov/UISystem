#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.MouseActions;
using VladislavTsurikov.MegaWorld.Runtime.Common;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.GUI.MouseActions
{
    [ElementEditor(typeof(Scale))]
    public class ScaleEditor : IMGUIElementEditor
    {
        private Scale _settings;

        public override void OnEnable() => _settings = (Scale)Target;

        public override void OnGUI()
        {
            _settings.MouseScaleSettings.MouseSensitivity = CustomEditorGUILayout.Slider(
                new GUIContent("Mouse Sensitivity"),
                _settings.MouseScaleSettings.MouseSensitivity, MouseSensitivitySettings.MinMouseSensitivity,
                MouseSensitivitySettings.MaxMouseSensitivity);
            _settings.EnableSnapScale =
                CustomEditorGUILayout.Toggle(new GUIContent("Enable Snap Scale"), _settings.EnableSnapScale);
            if (_settings.EnableSnapScale)
            {
                EditorGUI.indentLevel++;
                _settings.SnapScale =
                    Mathf.Max(CustomEditorGUILayout.FloatField(new GUIContent("Snap Scale"), _settings.SnapScale),
                        0.001f);
                EditorGUI.indentLevel--;
            }
        }
    }
}
#endif
