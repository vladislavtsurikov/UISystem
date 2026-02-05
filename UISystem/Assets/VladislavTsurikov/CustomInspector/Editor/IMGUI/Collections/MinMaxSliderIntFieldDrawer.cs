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
    public sealed class MinMaxSliderIntFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(FieldInfo field)
        {
            var attribute = field.GetCustomAttribute<MinMaxSliderAttribute>();
            return field.FieldType == typeof(int) &&
                   attribute != null &&
                   !string.IsNullOrWhiteSpace(attribute.MaxFieldName);
        }

        public override IReadOnlyList<Type> AttributeTypes => new[] { typeof(MinMaxSliderAttribute) };
        public override Type DrawerType => typeof(MinMaxSliderIntFieldDrawer);
    }

    public sealed class MinMaxSliderIntFieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            int intValue = value != null ? (int)value : 0;
            var minMaxAttribute = field.GetCustomAttribute<MinMaxSliderAttribute>();
            if (minMaxAttribute == null || string.IsNullOrWhiteSpace(minMaxAttribute.MaxFieldName))
            {
                return EditorGUI.IntField(rect, label, intValue);
            }

            if (!MinMaxSliderDrawer.TryGetPairedField(target, minMaxAttribute.MaxFieldName, out var maxField))
            {
                return EditorGUI.IntField(rect, label, intValue);
            }

            var maxValue = maxField.GetValue(target);
            float maxFloat = maxValue is int intMax ? intMax : intValue;
            float minFloat = intValue;

            MinMaxSliderDrawer.DrawSlider(
                rect,
                label,
                minMaxAttribute,
                ref minFloat,
                ref maxFloat,
                true,
                target);

            intValue = Mathf.RoundToInt(minFloat);
            maxField.SetValue(target, Mathf.RoundToInt(maxFloat));
            return intValue;
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






