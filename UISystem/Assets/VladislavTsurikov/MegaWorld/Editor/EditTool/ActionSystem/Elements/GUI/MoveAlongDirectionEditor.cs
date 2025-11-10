#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Common;

namespace VladislavTsurikov.MegaWorld.Editor.EditTool.ActionSystem.GUI
{
    [ElementEditor(typeof(MoveAlongDirection))]
    public class MoveAlongDirectionEditor : IMGUIElementEditor
    {
        private readonly GUIContent _mouseSensitivity = new("Mouse Sensitivity",
            "Сhanges the strength of the transform modification with the mouse.");

        private MoveAlongDirection _settings;

        public override void OnEnable() => _settings = (MoveAlongDirection)Target;

        public override void OnGUI() =>
            _settings.MouseSensitivitySettings.MouseSensitivity = CustomEditorGUILayout.Slider(_mouseSensitivity,
                _settings.MouseSensitivitySettings.MouseSensitivity,
                MouseSensitivitySettings.MinMouseSensitivity, MouseSensitivitySettings.MaxMouseSensitivity);
    }
}
#endif
