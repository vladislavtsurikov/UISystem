#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class MinMaxSliderVector3FieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(FieldInfo field)
        {
            var attribute = field.GetCustomAttribute<MinMaxSliderAttribute>();
            return field.FieldType == typeof(Vector3) &&
                   attribute != null &&
                   !string.IsNullOrWhiteSpace(attribute.MaxFieldName);
        }

        public override IReadOnlyList<Type> AttributeTypes => new[] { typeof(MinMaxSliderAttribute) };
        public override Type DrawerType => typeof(MinMaxSliderVector3FieldDrawer);
    }

    public sealed class MinMaxSliderVector3FieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            var vectorValue = value != null ? (Vector3)value : Vector3.zero;
            var minMaxAttribute = field.GetCustomAttribute<MinMaxSliderAttribute>();
            if (minMaxAttribute == null || string.IsNullOrWhiteSpace(minMaxAttribute.MaxFieldName))
            {
                return EditorGUI.Vector3Field(rect, label, vectorValue);
            }

            if (!MinMaxSliderDrawer.TryGetPairedField(target, minMaxAttribute.MaxFieldName, out var maxField))
            {
                return EditorGUI.Vector3Field(rect, label, vectorValue);
            }

            var maxValue = maxField.GetValue(target);
            var maxVector = maxValue is Vector3 vector ? vector : vectorValue;
            var isUniform = IsUniformMode(target, minMaxAttribute.UniformToggleFieldName);

            if (isUniform)
            {
                float minScale = vectorValue.x;
                float maxScale = maxVector.x;

                MinMaxSliderDrawer.DrawSlider(
                    rect,
                    label,
                    minMaxAttribute,
                    ref minScale,
                    ref maxScale,
                    false,
                    target);

                vectorValue = new Vector3(minScale, minScale, minScale);
                maxVector = new Vector3(maxScale, maxScale, maxScale);
            }
            else
            {
                var minLabel = new GUIContent(ObjectNames.NicifyVariableName(field.Name));
                vectorValue = EditorGUI.Vector3Field(rect, minLabel, vectorValue);
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                var maxLabel = new GUIContent(ObjectNames.NicifyVariableName(maxField.Name));
                maxVector = EditorGUI.Vector3Field(rect, maxLabel, maxVector);
            }

            maxField.SetValue(target, maxVector);
            return vectorValue;
        }

        public override float GetFieldsHeight(object target, FieldInfo field, object value)
        {
            var attribute = field?.GetCustomAttribute<MinMaxSliderAttribute>();
            if (attribute != null && !string.IsNullOrWhiteSpace(attribute.MaxFieldName))
            {
                var isUniform = IsUniformMode(target, attribute.UniformToggleFieldName);
                if (isUniform)
                {
                    return MinMaxSliderDrawer.GetHeight(attribute);
                }

                return EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing;
            }

            return base.GetFieldsHeight(target, field, value);
        }

        private static bool IsUniformMode(object target, string uniformToggleFieldName)
        {
            if (target == null || string.IsNullOrWhiteSpace(uniformToggleFieldName))
            {
                return true;
            }

            var field = target.GetType().GetField(uniformToggleFieldName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field == null)
            {
                return true;
            }

            return field.GetValue(target) is bool boolValue && boolValue;
        }
    }
}
#endif






