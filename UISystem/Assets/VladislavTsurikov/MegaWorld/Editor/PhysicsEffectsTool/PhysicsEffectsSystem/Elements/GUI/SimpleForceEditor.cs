#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Nody.Editor.Core;
using VladislavTsurikov.Core.Editor;
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
            _settings.Angle = EditorGUILayout.Slider(new GUIContent("Angle"), _settings.Angle, 0, 360);
            _settings.Force =
                EditorGUILayout.Slider(new GUIContent("Force"), _settings.Force, 0, _settings.MaxForce);
        }
    }
}
#endif
