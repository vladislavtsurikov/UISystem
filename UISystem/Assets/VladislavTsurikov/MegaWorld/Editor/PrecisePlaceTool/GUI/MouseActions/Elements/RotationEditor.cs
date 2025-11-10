#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.MouseActions;
using VladislavTsurikov.MegaWorld.Runtime.Common;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.GUI.MouseActions
{
    [ElementEditor(typeof(Rotation))]
    public class RotationEditor : IMGUIElementEditor
    {
        private Rotation _settings;

        public override void OnEnable() => _settings = (Rotation)Target;

        public override void OnGUI()
        {
            _settings.MouseRotationSettings.MouseSensitivity = CustomEditorGUILayout.Slider(
                new GUIContent("Mouse Sensitivity"), _settings.MouseRotationSettings.MouseSensitivity,
                MouseSensitivitySettings.MinMouseSensitivity, MouseSensitivitySettings.MaxMouseSensitivity);
            _settings.WayRotateY =
                (WayRotateY)CustomEditorGUILayout.EnumPopup(new GUIContent("Way Rotate Y"), _settings.WayRotateY);
            if (_settings.WayRotateY == WayRotateY.Offset)
            {
                GlobalCommonComponentSingleton<TransformSpaceSettings>.Instance.TransformSpace =
                    (TransformSpace)CustomEditorGUILayout.EnumPopup(new GUIContent("Transform Space"),
                        GlobalCommonComponentSingleton<TransformSpaceSettings>.Instance.TransformSpace);
            }

            _settings.EnableSnapRotate =
                CustomEditorGUILayout.Toggle(new GUIContent("Enable Snap Rotate"), _settings.EnableSnapRotate);
            if (_settings.EnableSnapRotate)
            {
                EditorGUI.indentLevel++;
                _settings.SnapRotate =
                    Mathf.Max(CustomEditorGUILayout.FloatField(new GUIContent("Snap Rotate"), _settings.SnapRotate),
                        0.001f);
                EditorGUI.indentLevel--;
            }
        }
    }
}
#endif
