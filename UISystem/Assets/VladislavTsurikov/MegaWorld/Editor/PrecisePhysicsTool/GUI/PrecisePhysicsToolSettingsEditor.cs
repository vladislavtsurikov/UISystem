#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePhysicsTool.GUI
{
    [DontDrawFoldout]
    [ElementEditor(typeof(PrecisePhysicsToolSettings))]
    public class PrecisePhysicsToolSettingsEditor : IMGUIElementEditor
    {
        private PrecisePhysicsToolSettings _precisePhysicsToolSettings;

        public override void OnEnable() => _precisePhysicsToolSettings = (PrecisePhysicsToolSettings)Target;

        public override void OnGUI()
        {
            _precisePhysicsToolSettings.PositionOffsetY =
                CustomEditorGUILayout.FloatField(new GUIContent("Position Offset Y"),
                    _precisePhysicsToolSettings.PositionOffsetY);
            _precisePhysicsToolSettings.Spacing =
                Mathf.Max(
                    CustomEditorGUILayout.FloatField(new GUIContent("Spacing"), _precisePhysicsToolSettings.Spacing),
                    0.5f);
        }
    }
}
#endif
