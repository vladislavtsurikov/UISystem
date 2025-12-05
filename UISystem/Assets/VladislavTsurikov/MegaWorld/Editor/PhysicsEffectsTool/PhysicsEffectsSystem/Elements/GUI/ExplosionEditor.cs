#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;

namespace VladislavTsurikov.MegaWorld.Editor.PhysicsEffectsTool.PhysicsEffectsSystem.GUI
{
    [ElementEditor(typeof(Explosion))]
    public class ExplosionEditor : PhysicsEffectEditor
    {
        private Explosion _settings;

        public override void OnEnable() => _settings = (Explosion)Target;

        protected override void OnPhysicsEffectGUI() =>
            _settings.Force =
                CustomEditorGUILayout.Slider(new GUIContent("Force"), _settings.Force, 0, _settings.MaxForce);
    }
}
#endif
