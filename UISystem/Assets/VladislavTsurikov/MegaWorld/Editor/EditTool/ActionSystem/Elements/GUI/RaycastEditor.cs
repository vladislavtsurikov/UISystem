#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;

namespace VladislavTsurikov.MegaWorld.Editor.EditTool.ActionSystem.GUI
{
    [ElementEditor(typeof(Raycast))]
    public class RaycastEditor : IMGUIElementEditor
    {
        private readonly GUIContent _groundLayersGUIContent = new("Ground Layers",
            "The Ray hits the object and detects objects around the hit ray.");

        private readonly GUIContent _raycastPositionOffsetGUIContent =
            new("Raycast Position Offset", "Adds Y position offset");

        private Raycast _settings;

        public override void OnEnable() => _settings = (Raycast)Target;

        public override void OnGUI()
        {
            _settings.RaycastPositionOffset =
                CustomEditorGUILayout.FloatField(_raycastPositionOffsetGUIContent, _settings.RaycastPositionOffset);
            _settings.GroundLayers = CustomEditorGUILayout.LayerField(_groundLayersGUIContent, _settings.GroundLayers);
        }
    }
}
#endif
