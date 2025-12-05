#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;

namespace VladislavTsurikov.MegaWorld.Editor.PhysicsEffectsTool.PhysicsEffectsSystem.GUI
{
    [ElementEditor(typeof(BlackHole))]
    public class BlackHoleEditor : PhysicsEffectEditor
    {
        private BlackHole _settings;

        public override void OnEnable() => _settings = (BlackHole)Target;

        protected override void OnPhysicsEffectGUI() =>
            _settings.Force =
                CustomEditorGUILayout.Slider(new GUIContent("Force"), _settings.Force, 0, _settings.MaxForce);
    }
}
#endif
