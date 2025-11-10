#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.TransformElementSystem
{
    [ElementEditor(typeof(SlopeScale))]
    public class SlopeScaleEditor : ReorderableListComponentEditor
    {
        private SlopeScale _slopeScale;

        public override void OnEnable() => _slopeScale = (SlopeScale)Target;

        public override void OnGUI(Rect rect, int index)
        {
            _slopeScale.UniformScaleOffset =
                EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Uniform Scale Offset"), _slopeScale.UniformScaleOffset);
            rect.y += EditorGUIUtility.singleLineHeight;

            _slopeScale.MaxSlope =
                EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Max Slope"), _slopeScale.MaxSlope);
            rect.y += EditorGUIUtility.singleLineHeight;

            if (_slopeScale.UniformScaleOffset)
            {
                _slopeScale.MaxUniformScaleOffset = EditorGUI.FloatField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Max Uniform Scale Offset"), _slopeScale.MaxUniformScaleOffset);
                rect.y += EditorGUIUtility.singleLineHeight;
            }
            else
            {
                _slopeScale.MaxScaleOffset = EditorGUI.Vector3Field(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Max Scale Offset"), _slopeScale.MaxScaleOffset);
                rect.y += EditorGUIUtility.singleLineHeight;
                rect.y += EditorGUIUtility.singleLineHeight;
            }
        }

        public override float GetElementHeight(int index)
        {
            var height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;

            if (_slopeScale.UniformScaleOffset)
            {
                height += EditorGUIUtility.singleLineHeight;
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
            }

            return height;
        }
    }
}
#endif
