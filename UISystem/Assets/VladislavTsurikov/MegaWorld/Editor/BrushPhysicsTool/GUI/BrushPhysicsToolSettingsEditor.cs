#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;

namespace VladislavTsurikov.MegaWorld.Editor.BrushPhysicsTool.GUI
{
    [DontDrawFoldout]
    [ElementEditor(typeof(BrushPhysicsToolSettings))]
    public class BrushPhysicsToolSettingsEditor : IMGUIElementEditor
    {
        private BrushPhysicsToolSettings _brushPhysicsToolSettings;

        public override void OnEnable() => _brushPhysicsToolSettings = (BrushPhysicsToolSettings)Target;

        public override void OnGUI() =>
            _brushPhysicsToolSettings.PositionOffsetY = Mathf.Max(0,
                CustomEditorGUILayout.FloatField(new GUIContent("Position Offset Y"),
                    _brushPhysicsToolSettings.PositionOffsetY));
    }
}
#endif
