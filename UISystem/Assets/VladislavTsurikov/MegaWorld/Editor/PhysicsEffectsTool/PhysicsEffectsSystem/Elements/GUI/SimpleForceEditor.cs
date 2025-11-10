#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;

namespace VladislavTsurikov.MegaWorld.Editor.PhysicsEffectsTool.PhysicsEffectsSystem.GUI
{
    [ElementEditor(typeof(SimpleForce))]
    public class SimpleForceEditor : PhysicsEffectEditor
    {
        private SimpleForce _settings;

        public override void OnEnable() => _settings = (SimpleForce)Target;

        protected override void OnPhysicsEffectGUI()
        {
            _settings.Angle = CustomEditorGUILayout.Slider(new GUIContent("Angle"), _settings.Angle, 0, 360);
            _settings.Force =
                CustomEditorGUILayout.Slider(new GUIContent("Force"), _settings.Force, 0, _settings.MaxForce);
        }
    }
}
#endif
