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
    [ElementEditor(typeof(MoveAlongDirection))]
    public class MoveAlongDirectionEditor : IMGUIElementEditor
    {
        private MoveAlongDirection _settings;

        public override void OnEnable() => _settings = (MoveAlongDirection)Target;

        public override void OnGUI()
        {
            _settings.MouseMoveAlongDirectionSettings.MouseSensitivity = CustomEditorGUILayout.Slider(
                new GUIContent("Mouse Sensitivity"), _settings.MouseMoveAlongDirectionSettings.MouseSensitivity,
                MouseSensitivitySettings.MinMouseSensitivity, MouseSensitivitySettings.MaxMouseSensitivity);

            _settings.MoveAlongAxis =
                (MoveAlongAxis)CustomEditorGUILayout.EnumPopup(new GUIContent("Move Along Axis"),
                    _settings.MoveAlongAxis);

            if (_settings.MoveAlongAxis == MoveAlongAxis.SurfaceNormal)
            {
                EditorGUI.indentLevel++;

                _settings.WeightToNormal = CustomEditorGUILayout.Slider(new GUIContent("Weight To Normal"),
                    _settings.WeightToNormal, 0, 1);

                EditorGUI.indentLevel--;
            }
        }
    }
}
#endif
