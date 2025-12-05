#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.TransformElementSystem
{
    [Serializable]
    [ElementEditor(typeof(Align))]
    public class AlignEditor : ReorderableListComponentEditor
    {
        private Align _align;

        public override void OnEnable() => _align = (Align)Target;

        public override void OnGUI(Rect rect, int index)
        {
            var alignmentStyleRight = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleRight, stretchWidth = true
            };
            var alignmentStyleLeft = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft, stretchWidth = true
            };

            _align.UseNormalWeight =
                EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Use Normal Weight"), _align.UseNormalWeight);
            rect.y += EditorGUIUtility.singleLineHeight;

            if (_align.UseNormalWeight)
            {
                _align.MinMaxRange =
                    EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        new GUIContent("Min Max Range"), _align.MinMaxRange);
                rect.y += EditorGUIUtility.singleLineHeight;

                if (_align.MinMaxRange)
                {
                    EditorGUI.MinMaxSlider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        new GUIContent("Weight To Normal"), ref _align.MinWeightToNormal, ref _align.MaxWeightToNormal,
                        0, 1);
                    rect.y += EditorGUIUtility.singleLineHeight;

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

                    _align.MinWeightToNormal = EditorGUI.FloatField(numFieldRect, _align.MinWeightToNormal);
                    numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width,
                        EditorGUIUtility.singleLineHeight);

                    EditorGUI.LabelField(numFieldRect, " ");
                    numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width,
                        EditorGUIUtility.singleLineHeight);

                    _align.MaxWeightToNormal = EditorGUI.FloatField(numFieldRect, _align.MaxWeightToNormal);
                    numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width,
                        EditorGUIUtility.singleLineHeight);

                    var maxContent = new GUIContent("");
                    EditorGUI.LabelField(numFieldRect, maxContent, alignmentStyleRight);
                }
                else
                {
                    _align.MaxWeightToNormal =
                        EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                            new GUIContent("Weight To Normal"), _align.MaxWeightToNormal, 0, 1);
                    rect.y += EditorGUIUtility.singleLineHeight;
                }
            }
        }

        public override float GetElementHeight(int index)
        {
            var height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;

            if (_align.UseNormalWeight)
            {
                height += EditorGUIUtility.singleLineHeight;

                if (_align.MinMaxRange)
                {
                    height += EditorGUIUtility.singleLineHeight;
                    height += EditorGUIUtility.singleLineHeight;
                }
                else
                {
                    height += EditorGUIUtility.singleLineHeight;
                }
            }

            return height;
        }
    }
}
#endif
