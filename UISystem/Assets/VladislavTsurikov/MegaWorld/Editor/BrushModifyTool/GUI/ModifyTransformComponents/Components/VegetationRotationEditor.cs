#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.GUI.ModifyTransformComponents
{
    [ElementEditor(typeof(VegetationRotation))]
    public class VegetationRotationEditor : ReorderableListComponentEditor
    {
        private VegetationRotation _settings;

        public override void OnEnable() => _settings = (VegetationRotation)Target;

        public override void OnGUI(Rect rect, int index)
        {
            _settings.StrengthY =
                EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Strength Y (%)", "How much the object will rotate along the Y axis."),
                    _settings.StrengthY, 0.0f, 20.0f);
            rect.y += EditorGUIUtility.singleLineHeight;
            _settings.StrengthXY =
                EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Strength Rotation XY", "How quickly will this effect work"), _settings.StrengthXY,
                    0.0f, 100.0f);
            rect.y += EditorGUIUtility.singleLineHeight;
            _settings.RotationXZ = EditorGUI.Slider(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Rotation XZ (%)",
                    "The strength of the rotation along the Y and X axes. The larger this value, the more trees will be fallen. 1 - fallen tree, 0 - no."),
                _settings.RotationXZ, 0.0f, 20.0f);
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index)
        {
            var height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight * 3;

            return height;
        }
    }
}
#endif
