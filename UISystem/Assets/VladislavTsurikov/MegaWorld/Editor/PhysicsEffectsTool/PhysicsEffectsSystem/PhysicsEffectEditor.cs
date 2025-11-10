#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
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
            _settings.PositionOffsetY = CustomEditorGUILayout.Slider(new GUIContent("Position Offset Y"),
                _settings.PositionOffsetY, -20, 20);
            _settings.Size = CustomEditorGUILayout.Slider(new GUIContent("Size"), _settings.Size, 0, 100);

            OnPhysicsEffectGUI();
        }

        protected virtual void OnPhysicsEffectGUI()
        {
        }
    }
}
#endif
