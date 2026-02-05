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
    public sealed class MinMaxSliderFloatFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(FieldInfo field)
        {
            var attribute = field.GetCustomAttribute<MinMaxSliderAttribute>();
            return field.FieldType == typeof(float) &&
                   attribute != null &&
                   !string.IsNullOrWhiteSpace(attribute.MaxFieldName);
        }

        public override IReadOnlyList<Type> AttributeTypes => new[] { typeof(MinMaxSliderAttribute) };
        public override Type DrawerType => typeof(MinMaxSliderFloatFieldDrawer);
    }

    public sealed class MinMaxSliderFloatFieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            float floatValue = value != null ? (float)value : 0f;
            var minMaxAttribute = field.GetCustomAttribute<MinMaxSliderAttribute>();
            if (minMaxAttribute == null || string.IsNullOrWhiteSpace(minMaxAttribute.MaxFieldName))
            {
                return EditorGUI.FloatField(rect, label, floatValue);
            }

            if (!MinMaxSliderDrawer.TryGetPairedField(target, minMaxAttribute.MaxFieldName, out var maxField))
            {
                return EditorGUI.FloatField(rect, label, floatValue);
            }

            var maxValue = maxField.GetValue(target);
            var maxFloat = maxValue is float floatMax ? floatMax : floatValue;

            MinMaxSliderDrawer.DrawSlider(
                rect,
                label,
                minMaxAttribute,
                ref floatValue,
                ref maxFloat,
                false,
                target);

            maxField.SetValue(target, maxFloat);
            return floatValue;
        }

        public override float GetFieldsHeight(object target, FieldInfo field, object value)
        {
            var attribute = field?.GetCustomAttribute<MinMaxSliderAttribute>();
            if (attribute != null && !string.IsNullOrWhiteSpace(attribute.MaxFieldName))
            {
                return MinMaxSliderDrawer.GetHeight(attribute);
            }

            return base.GetFieldsHeight(target, field, value);
        }
    }
}
#endif






