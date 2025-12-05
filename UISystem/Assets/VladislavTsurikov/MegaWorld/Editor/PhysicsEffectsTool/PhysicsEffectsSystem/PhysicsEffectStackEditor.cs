#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;

namespace VladislavTsurikov.MegaWorld.Editor.PhysicsEffectsTool.PhysicsEffectsSystem
{
    public class PhysicsEffectStackEditor : TabComponentStackEditor<PhysicsEffect, PhysicsEffectEditor>
    {
        private bool _effectSettingsFoldout = true;

        public PhysicsEffectStackEditor(ComponentStackOnlyDifferentTypes<PhysicsEffect> list) : base(list)
        {
            _tabStackEditor.TabWidthFromName = true;
            _tabStackEditor.Draggable = true;
        }

        protected override void OnSelectedComponentGUI()
        {
            _effectSettingsFoldout = CustomEditorGUILayout.Foldout(_effectSettingsFoldout,
                "Effect Settings" + " (" + Stack.SelectedElement?.Name + ")");

            if (_effectSettingsFoldout)
            {
                EditorGUI.indentLevel++;

                if (Stack.SelectedElement == null)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.LabelField("No Physics Effect Selected");
                    EditorGUILayout.EndVertical();
                }
                else
                {
                    SelectedEditor?.OnGUI();
                }

                EditorGUI.indentLevel--;
            }
        }

        public void DrawButtons() => _tabStackEditor.OnGUI();
    }
}
#endif
