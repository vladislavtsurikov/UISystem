#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class Vector3FieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(Type fieldType) => fieldType == typeof(Vector3);
        public override Type DrawerType => typeof(Vector3FieldDrawer);
    }

    public class Vector3FieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object value)
        {
            var vectorValue = value != null ? (Vector3)value : Vector3.zero;
            var minMaxAttribute = field.GetCustomAttribute<MinMaxSliderAttribute>();
            if (minMaxAttribute != null && !string.IsNullOrWhiteSpace(minMaxAttribute.MaxFieldName))
            {
                var context = InspectorContext.Current;
                if (MinMaxSliderDrawer.TryGetPairedField(context?.Target, minMaxAttribute.MaxFieldName,
                        out var maxField))
                {
                    var maxValue = maxField.GetValue(context.Target);
                    var maxVector = maxValue is Vector3 vector ? vector : vectorValue;
                    var isUniform = IsUniformMode(context?.Target, minMaxAttribute.UniformToggleFieldName);

                    if (isUniform)
                    {
                        float minScale = vectorValue.x;
                        float maxScale = maxVector.x;

                        MinMaxSliderDrawer.DrawSlider(rect, label, minMaxAttribute, ref minScale, ref maxScale, false);

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

                    maxField.SetValue(context.Target, maxVector);
                    return vectorValue;
                }
            }

            return EditorGUI.Vector3Field(rect, label, vectorValue);
        }

        public override float GetFieldsHeight(object target)
        {
            var context = InspectorContext.Current;
            var attribute = context?.Field?.GetCustomAttribute<MinMaxSliderAttribute>();
            if (attribute != null && !string.IsNullOrWhiteSpace(attribute.MaxFieldName))
            {
                var isUniform = IsUniformMode(context?.Target, attribute.UniformToggleFieldName);
                if (isUniform)
                {
                    return MinMaxSliderDrawer.GetHeight(attribute);
                }

                return EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing;
            }

            return base.GetFieldsHeight(target);
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
