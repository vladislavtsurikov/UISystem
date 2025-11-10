#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;

namespace VladislavTsurikov.MegaWorld.Editor.PhysicsEffectsTool.PhysicsEffectsSystem.GUI
{
    [ElementEditor(typeof(FlyUp))]
    public class FlyUpEditor : PhysicsEffectEditor
    {
        private FlyUp _settings;

        public override void OnEnable() => _settings = (FlyUp)Target;

        protected override void OnPhysicsEffectGUI() =>
            _settings.Force =
                CustomEditorGUILayout.Slider(new GUIContent("Force"), _settings.Force, 0, _settings.MaxForce);
    }
}
#endif
