#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Editor.PhysicsEffectsTool.PhysicsEffectsSystem;

namespace VladislavTsurikov.MegaWorld.Editor.PhysicsEffectsTool.GUI
{
    [DontDrawFoldout]
    [ElementEditor(typeof(PhysicsEffectsToolSettings))]
    public class PhysicsEffectsToolSettingsEditor : IMGUIElementEditor
    {
        private PhysicsEffectsToolSettings _physicsEffectsToolSettings;

        public PhysicsEffectStackEditor PhysicsEffectStackEditor;

        public override void OnEnable()
        {
            _physicsEffectsToolSettings = (PhysicsEffectsToolSettings)Target;
            PhysicsEffectStackEditor = new PhysicsEffectStackEditor(_physicsEffectsToolSettings.List);
            PhysicsEffectStackEditor.RefreshEditors();
        }

        public override void OnGUI()
        {
            _physicsEffectsToolSettings.Spacing =
                Mathf.Max(
                    CustomEditorGUILayout.FloatField(new GUIContent("Spacing"), _physicsEffectsToolSettings.Spacing),
                    0.5f);
            PhysicsEffectStackEditor.DrawSelectedSettings();
        }
    }
}
#endif
