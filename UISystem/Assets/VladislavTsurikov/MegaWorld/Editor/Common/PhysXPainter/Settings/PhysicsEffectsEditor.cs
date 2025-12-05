#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter.Settings;

namespace VladislavTsurikov.MegaWorld.Editor.Common.PhysXPainter.Settings
{
    [ElementEditor(typeof(PhysicsEffects))]
    public class PhysicsEffectsEditor : IMGUIElementEditor
    {
        public bool DirectionFoldout = true;

        public bool ForceFoldout = true;
        private PhysicsEffects _element => (PhysicsEffects)Target;

        public override void OnGUI()
        {
            ForceFoldout = CustomEditorGUILayout.Foldout(ForceFoldout, "Force");

            if (ForceFoldout)
            {
                EditorGUI.indentLevel++;

                _element.ForceRange = CustomEditorGUILayout.Toggle(new GUIContent("Force Range"), _element.ForceRange);

                if (_element.ForceRange)
                {
                    _element.MinForce = Mathf.Max(0,
                        CustomEditorGUILayout.Slider(new GUIContent("Min Force"), _element.MinForce, 0, 100));
                    _element.MaxForce = Mathf.Max(_element.MinForce,
                        CustomEditorGUILayout.Slider(new GUIContent("Max Force"), _element.MaxForce, 0, 100));
                }
                else
                {
                    _element.MinForce = Mathf.Max(0,
                        CustomEditorGUILayout.Slider(new GUIContent("Force"), _element.MinForce, 0, 100));
                }

                EditorGUI.indentLevel--;
            }

            DirectionFoldout = CustomEditorGUILayout.Foldout(DirectionFoldout, "Direction");

            if (DirectionFoldout)
            {
                EditorGUI.indentLevel++;

                _element.RandomStrength = CustomEditorGUILayout.Slider(new GUIContent("Random Strength"),
                    _element.RandomStrength, 0, 100);

                EditorGUI.indentLevel--;
            }
        }
    }
}
#endif
