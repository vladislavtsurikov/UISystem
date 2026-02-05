#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Nody.Editor.Core;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;

namespace VladislavTsurikov.MegaWorld.Editor.PhysicsEffectsTool.PhysicsEffectsSystem
{
    [ElementEditor(typeof(PhysicsEffect))]
    public class PhysicsEffectEditor : IMGUIElementEditor
    {
        private PhysicsEffect _settings => (PhysicsEffect)Target;

        public override void OnGUI()
        {
            _settings.PositionOffsetY = EditorGUILayout.Slider(new GUIContent("Position Offset Y"),
                _settings.PositionOffsetY, -20, 20);
            _settings.Size = EditorGUILayout.Slider(new GUIContent("Size"), _settings.Size, 0, 100);

            OnPhysicsEffectGUI();
        }

        protected virtual void OnPhysicsEffectGUI()
        {
        }
    }
}
#endif
