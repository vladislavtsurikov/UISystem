#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public static class MinMaxSliderDrawer
    {
        public static float GetHeight(MinMaxSliderAttribute attribute)
        {
            var height = EditorGUIUtility.singleLineHeight;

            if (HasLabels(attribute))
            {
                height += EditorGUIUtility.singleLineHeight;
            }

            if (attribute.ShowFields)
            {
                height += EditorGUIUtility.singleLineHeight;
            }

            return height;
        }

        public static void DrawSlider(
            Rect rect,
            GUIContent label,
            MinMaxSliderAttribute attribute,
            ref float minValue,
            ref float maxValue,
            bool useIntFields,
            object target)
        {
            var labelContent = string.IsNullOrWhiteSpace(attribute.LabelOverride)
                ? label
                : new GUIContent(attribute.LabelOverride, label.tooltip);

            var minLimit = attribute.Min;
            var maxLimit = GetLimitValue(attribute.Max, attribute.Max, attribute.MaxValueMemberName, target);

            var lineHeight = EditorGUIUtility.singleLineHeight;
            var sliderRect = new Rect(rect.x, rect.y, rect.width, lineHeight);

            EditorGUI.MinMaxSlider(sliderRect, labelContent, ref minValue, ref maxValue, minLimit, maxLimit);

            rect.y += lineHeight;

            if (HasLabels(attribute))
            {
                DrawLabels(rect, attribute);
                rect.y += lineHeight;
            }

            if (attribute.ShowFields)
            {
                DrawFields(rect, ref minValue, ref maxValue, useIntFields);
            }

            ClampMinMax(ref minValue, ref maxValue, minLimit, maxLimit, useIntFields);
        }

        public static bool TryGetPairedField(object target, string fieldName, out FieldInfo field)
        {
            field = null;
            if (target == null || string.IsNullOrWhiteSpace(fieldName))
            {
                return false;
            }

            field = target.GetType().GetField(fieldName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return field != null;
        }

        private static bool HasLabels(MinMaxSliderAttribute attribute)
        {
            return attribute.LabelPreset != MinMaxSliderLabelPreset.None ||
                   !string.IsNullOrWhiteSpace(attribute.LabelLeft) ||
                   !string.IsNullOrWhiteSpace(attribute.LabelCenter) ||
                   !string.IsNullOrWhiteSpace(attribute.LabelRight);
        }

        private static void DrawLabels(Rect rect, MinMaxSliderAttribute attribute)
        {
            var labelWidth = EditorGUIUtility.labelWidth;
            var contentWidth = rect.width - labelWidth;
            var segmentWidth = contentWidth * 0.2f;

            var leftLabel = new Rect(rect.x + labelWidth, rect.y, segmentWidth, rect.height);
            var centerLabel = new Rect(rect.x + labelWidth + segmentWidth, rect.y, segmentWidth * 3f, rect.height);
            var rightLabel = new Rect(rect.x + labelWidth + segmentWidth * 4f, rect.y, segmentWidth, rect.height);

            var alignmentStyleLeft = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft, stretchWidth = true
            };
            var alignmentStyleCenter = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter, stretchWidth = true
            };
            var alignmentStyleRight = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleRight, stretchWidth = true
            };

            var (left, center, right) = GetLabelTexts(attribute);

            EditorGUI.LabelField(leftLabel, left ?? string.Empty, alignmentStyleLeft);
            EditorGUI.LabelField(centerLabel, center ?? string.Empty, alignmentStyleCenter);
            EditorGUI.LabelField(rightLabel, right ?? string.Empty, alignmentStyleRight);
        }

        private static void DrawFields(Rect rect, ref float minValue, ref float maxValue, bool useIntFields)
        {
            var labelRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, rect.height);
            EditorGUI.LabelField(labelRect, GUIContent.none);

            var contentWidth = rect.width - EditorGUIUtility.labelWidth;
            var segmentWidth = contentWidth * 0.2f;

            var minFieldRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, segmentWidth, rect.height);
            var spacerRect = new Rect(minFieldRect.x + segmentWidth, rect.y, segmentWidth, rect.height);
            var maxFieldRect = new Rect(spacerRect.x + segmentWidth, rect.y, segmentWidth, rect.height);

            if (useIntFields)
            {
                minValue = EditorGUI.IntField(minFieldRect, Mathf.RoundToInt(minValue));
                EditorGUI.LabelField(spacerRect, " ");
                maxValue = EditorGUI.IntField(maxFieldRect, Mathf.RoundToInt(maxValue));
            }
            else
            {
                minValue = EditorGUI.FloatField(minFieldRect, minValue);
                EditorGUI.LabelField(spacerRect, " ");
                maxValue = EditorGUI.FloatField(maxFieldRect, maxValue);
            }
        }

        private static (string left, string center, string right) GetLabelTexts(MinMaxSliderAttribute attribute)
        {
            if (!string.IsNullOrWhiteSpace(attribute.LabelLeft) ||
                !string.IsNullOrWhiteSpace(attribute.LabelCenter) ||
                !string.IsNullOrWhiteSpace(attribute.LabelRight))
            {
                return (attribute.LabelLeft, attribute.LabelCenter, attribute.LabelRight);
            }

            return attribute.LabelPreset switch
            {
                MinMaxSliderLabelPreset.ScaleZeroToFive => ("0", "2.5", "5"),
                MinMaxSliderLabelPreset.SlopeDegrees => ("0°", "45°", "90°"),
                _ => (string.Empty, string.Empty, string.Empty)
            };
        }

        private static void ClampMinMax(
            ref float minValue,
            ref float maxValue,
            float minLimit,
            float maxLimit,
            bool useIntFields)
        {
            minValue = Mathf.Clamp(minValue, minLimit, maxLimit);
            maxValue = Mathf.Clamp(maxValue, minLimit, maxLimit);

            if (useIntFields)
            {
                minValue = Mathf.RoundToInt(minValue);
                maxValue = Mathf.RoundToInt(maxValue);
            }
        }

        private static float GetLimitValue(
            float fallback,
            float defaultValue,
            string memberName,
            object target)
        {
            if (string.IsNullOrWhiteSpace(memberName))
            {
                return fallback;
            }

            if (target == null)
            {
                return fallback;
            }

            var targetType = target.GetType();
            var field = targetType.GetField(memberName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null)
            {
                return ConvertToFloat(field.GetValue(target), fallback);
            }

            var property = targetType.GetProperty(memberName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property != null)
            {
                return ConvertToFloat(property.GetValue(target), fallback);
            }

            return defaultValue;
        }

        private static float ConvertToFloat(object value, float fallback)
        {
            if (value is float floatValue)
            {
                return floatValue;
            }

            if (value is int intValue)
            {
                return intValue;
            }

            return fallback;
        }
    }
}
#endif
