#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Nody.Editor.Core;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;

namespace VladislavTsurikov.MegaWorld.Editor.EditTool.ActionSystem.GUI
{
    [ElementEditor(typeof(Rotate))]
    public class RotateEditor : IMGUIElementEditor
    {
        private readonly GUIContent _mouseSensitivity = new("Mouse Sensitivity",
            "Сhanges the strength of the transform modification with the mouse.");

        private readonly GUIContent _transformSpace = new("Transform Space", "Changes from local to global space.");
        private Rotate _settings;

        public override void OnEnable() => _settings = (Rotate)Target;

        public override void OnGUI()
        {
            _settings.MouseSensitivitySettings.MouseSensitivity = EditorGUILayout.Slider(_mouseSensitivity,
                _settings.MouseSensitivitySettings.MouseSensitivity,
                MouseSensitivitySettings.MinMouseSensitivity, MouseSensitivitySettings.MaxMouseSensitivity);

            GlobalCommonComponentSingleton<TransformSpaceSettings>.Instance.TransformSpace =
                (TransformSpace)EditorGUILayout.EnumPopup(_transformSpace,
                    GlobalCommonComponentSingleton<TransformSpaceSettings>.Instance.TransformSpace);

            _settings.EnableSnapRotate =
                EditorGUILayout.Toggle(new GUIContent("Enable Snap Rotate"), _settings.EnableSnapRotate);
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
