#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;

namespace VladislavTsurikov.MegaWorld.Editor.ExplodePhysics
{
    [DontDrawFoldout]
    [ElementEditor(typeof(ExplodePhysicsToolSettings))]
    public class ExplodePhysicsToolSettingsEditor : IMGUIElementEditor
    {
        private ExplodePhysicsToolSettings _explodePhysicsToolSettings;

        public override void OnEnable() => _explodePhysicsToolSettings = (ExplodePhysicsToolSettings)Target;

        public override void OnGUI()
        {
            _explodePhysicsToolSettings.PositionOffsetY =
                CustomEditorGUILayout.FloatField(new GUIContent("Position Offset Y"),
                    _explodePhysicsToolSettings.PositionOffsetY);
            _explodePhysicsToolSettings.Spacing =
                Mathf.Max(
                    CustomEditorGUILayout.FloatField(new GUIContent("Spacing"), _explodePhysicsToolSettings.Spacing),
                    0.5f);

            CustomEditorGUILayout.MinMaxIntSlider(new GUIContent("Instances"),
                ref _explodePhysicsToolSettings.InstancesMin, ref _explodePhysicsToolSettings.InstancesMax, 2, 200);
            _explodePhysicsToolSettings.SpawnFromOnePoint =
                CustomEditorGUILayout.Toggle(new GUIContent("Spawn From One Point"),
                    _explodePhysicsToolSettings.SpawnFromOnePoint);

            if (!_explodePhysicsToolSettings.SpawnFromOnePoint)
            {
                EditorGUI.indentLevel++;
                _explodePhysicsToolSettings.Size = CustomEditorGUILayout.Slider(new GUIContent("Size"),
                    _explodePhysicsToolSettings.Size, 10, 300);
                _explodePhysicsToolSettings.Force = CustomEditorGUILayout.Slider(new GUIContent("Force"),
                    _explodePhysicsToolSettings.Force, 0, 100);
                EditorGUI.indentLevel--;
            }
        }
    }
}
#endif
