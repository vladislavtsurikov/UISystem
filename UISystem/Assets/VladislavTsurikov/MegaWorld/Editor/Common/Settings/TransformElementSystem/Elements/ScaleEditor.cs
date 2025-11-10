#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.TransformElementSystem
{
    [ElementEditor(typeof(Scale))]
    public class ScaleEditor : ReorderableListComponentEditor
    {
        private Scale _scale;

        public override void OnEnable() => _scale = (Scale)Target;

        public override void OnGUI(Rect rect, int index)
        {
            _scale.UniformScale =
                EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Uniform Scale"), _scale.UniformScale);
            rect.y += EditorGUIUtility.singleLineHeight;

            if (_scale.UniformScale)
            {
                var minSameScaleValue = _scale.MinScale.x;
                var maxSameScaleValue = _scale.MaxScale.x;

                var alignmentStyleRight = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleRight, stretchWidth = true
                };
                var alignmentStyleLeft = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleLeft, stretchWidth = true
                };
                var alignmentStyleCenter = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter, stretchWidth = true
                };

                EditorGUI.MinMaxSlider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Scale"), ref minSameScaleValue, ref maxSameScaleValue, 0f, 5f);
                rect.y += EditorGUIUtility.singleLineHeight * 0.5f;
                var slopeLabelRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y,
                    (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(slopeLabelRect, "0", alignmentStyleLeft);
                slopeLabelRect =
                    new Rect(rect.x + EditorGUIUtility.labelWidth + (rect.width - EditorGUIUtility.labelWidth) * 0.2f,
                        rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.6f, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(slopeLabelRect, "2.5", alignmentStyleCenter);
                slopeLabelRect =
                    new Rect(rect.x + EditorGUIUtility.labelWidth + (rect.width - EditorGUIUtility.labelWidth) * 0.8f,
                        rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(slopeLabelRect, "5", alignmentStyleRight);
                rect.y += EditorGUIUtility.singleLineHeight;

                //Label
                EditorGUI.LabelField(
                    new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight),
                    new GUIContent(""));
                //Min Label
                var numFieldRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y,
                    (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
                var minContent = new GUIContent("");

                EditorGUI.LabelField(numFieldRect, minContent, alignmentStyleLeft);
                numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width,
                    EditorGUIUtility.singleLineHeight);

                minSameScaleValue = EditorGUI.FloatField(numFieldRect, minSameScaleValue);
                numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width,
                    EditorGUIUtility.singleLineHeight);

                EditorGUI.LabelField(numFieldRect, " ");
                numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width,
                    EditorGUIUtility.singleLineHeight);

                maxSameScaleValue = EditorGUI.FloatField(numFieldRect, maxSameScaleValue);
                numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width,
                    EditorGUIUtility.singleLineHeight);

                var maxContent = new GUIContent("");
                EditorGUI.LabelField(numFieldRect, maxContent, alignmentStyleRight);

                rect.y += EditorGUIUtility.singleLineHeight;

                _scale.MinScale = new Vector3(minSameScaleValue, minSameScaleValue, minSameScaleValue);
                _scale.MaxScale = new Vector3(maxSameScaleValue, maxSameScaleValue, maxSameScaleValue);
            }
            else
            {
                _scale.MinScale =
                    EditorGUI.Vector3Field(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        new GUIContent("Min Scale"), _scale.MinScale);
                rect.y += EditorGUIUtility.singleLineHeight;
                rect.y += EditorGUIUtility.singleLineHeight;
                _scale.MaxScale =
                    EditorGUI.Vector3Field(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        new GUIContent("Max Scale"), _scale.MaxScale);
                rect.y += EditorGUIUtility.singleLineHeight;
                rect.y += EditorGUIUtility.singleLineHeight;
            }
        }

        public override float GetElementHeight(int index)
        {
            var height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;

            if (_scale.UniformScale)
            {
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
            }

            return height;
        }
    }
}
#endif
