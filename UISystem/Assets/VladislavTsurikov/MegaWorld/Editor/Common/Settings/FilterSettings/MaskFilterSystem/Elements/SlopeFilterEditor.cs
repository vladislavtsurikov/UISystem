#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem
{
    [ElementEditor(typeof(SlopeFilter))]
    public class SlopeFilterEditor : MaskFilterEditor
    {
        private SlopeFilter _slopeFilter;

        public override void OnEnable() => _slopeFilter = (SlopeFilter)Target;

        public override void OnGUI(Rect rect, int index)
        {
            if (Terrain.activeTerrain == null)
            {
                EditorGUI.HelpBox(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    "There is no terrain in the scene", MessageType.Warning);

                rect.y += EditorGUIUtility.singleLineHeight;
                return;
            }

            if (Terrain.activeTerrain.drawInstanced == false)
            {
                EditorGUI.HelpBox(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    "Turn on Draw Instanced in all terrains, because this filter requires Normal Map",
                    MessageType.Warning);

                rect.y += EditorGUIUtility.singleLineHeight;
                return;
            }

            if (index != 0)
            {
                _slopeFilter.BlendMode = (BlendMode)EditorGUI.EnumPopup(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Blend Mode"), _slopeFilter.BlendMode);
                rect.y += EditorGUIUtility.singleLineHeight;
            }

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
                new GUIContent("Slope"), ref _slopeFilter.MinSlope, ref _slopeFilter.MaxSlope, 0f, 90);
            rect.y += EditorGUIUtility.singleLineHeight * 0.5f;
            var slopeLabelRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y,
                (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(slopeLabelRect, "0°", alignmentStyleLeft);
            slopeLabelRect =
                new Rect(rect.x + EditorGUIUtility.labelWidth + (rect.width - EditorGUIUtility.labelWidth) * 0.2f,
                    rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.6f, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(slopeLabelRect, "45°", alignmentStyleCenter);
            slopeLabelRect =
                new Rect(rect.x + EditorGUIUtility.labelWidth + (rect.width - EditorGUIUtility.labelWidth) * 0.8f,
                    rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(slopeLabelRect, "90°", alignmentStyleRight);
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

            _slopeFilter.MinSlope = EditorGUI.FloatField(numFieldRect, _slopeFilter.MinSlope);
            numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width,
                EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(numFieldRect, " ");
            numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width,
                EditorGUIUtility.singleLineHeight);

            _slopeFilter.MaxSlope = EditorGUI.FloatField(numFieldRect, _slopeFilter.MaxSlope);
            numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width,
                EditorGUIUtility.singleLineHeight);

            var maxContent = new GUIContent("");
            EditorGUI.LabelField(numFieldRect, maxContent, alignmentStyleRight);

            rect.y += EditorGUIUtility.singleLineHeight;

            _slopeFilter.SlopeFalloffType = (FalloffType)EditorGUI.EnumPopup(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Slope Falloff Type"), _slopeFilter.SlopeFalloffType);

            rect.y += EditorGUIUtility.singleLineHeight;

            if (_slopeFilter.SlopeFalloffType != FalloffType.None)
            {
                _slopeFilter.SlopeFalloffMinMax = EditorGUI.Toggle(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Slope Falloff Min Max"), _slopeFilter.SlopeFalloffMinMax);

                rect.y += EditorGUIUtility.singleLineHeight;

                if (_slopeFilter.SlopeFalloffMinMax)
                {
                    _slopeFilter.MinAddSlopeFalloff = Mathf.Max(0.1f,
                        EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                            new GUIContent("Min Add Slope Falloff"), _slopeFilter.MinAddSlopeFalloff));

                    rect.y += EditorGUIUtility.singleLineHeight;

                    _slopeFilter.MaxAddSlopeFalloff = Mathf.Max(0.1f,
                        EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                            new GUIContent("Max Add Slope Falloff"), _slopeFilter.MaxAddSlopeFalloff));
                }
                else
                {
                    _slopeFilter.AddSlopeFalloff = Mathf.Max(0.1f,
                        EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                            new GUIContent("Add Slope Falloff"), _slopeFilter.AddSlopeFalloff));
                }
            }
        }

        public override float GetElementHeight(int index)
        {
            var height = EditorGUIUtility.singleLineHeight;

            if (Terrain.activeTerrain == null)
            {
                height += EditorGUIUtility.singleLineHeight;
                return height;
            }

            if (Terrain.activeTerrain.drawInstanced == false)
            {
                height += EditorGUIUtility.singleLineHeight;
                return height;
            }

            if (index != 0)
            {
                height += EditorGUIUtility.singleLineHeight;
            }

            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;

            if (_slopeFilter.SlopeFalloffType != FalloffType.None)
            {
                height += EditorGUIUtility.singleLineHeight;

                if (_slopeFilter.SlopeFalloffMinMax)
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
