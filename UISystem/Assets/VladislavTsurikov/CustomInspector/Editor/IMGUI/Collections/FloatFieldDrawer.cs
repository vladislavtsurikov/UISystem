#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Runtime;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class FloatFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(Type fieldType) => fieldType == typeof(float);
        public override Type DrawerType => typeof(FloatFieldDrawer);
    }

    public class FloatFieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object value)
        {
            float floatValue = value != null ? (float)value : 0f;

            var minMaxAttribute = field.GetCustomAttribute<MinMaxSliderAttribute>();
            if (minMaxAttribute != null && !string.IsNullOrWhiteSpace(minMaxAttribute.MaxFieldName))
            {
                var context = InspectorContext.Current;
                if (MinMaxSliderDrawer.TryGetPairedField(context?.Target, minMaxAttribute.MaxFieldName,
                        out var maxField))
                {
                    var maxValue = maxField.GetValue(context.Target);
                    var maxFloat = maxValue is float floatMax ? floatMax : floatValue;

                    MinMaxSliderDrawer.DrawSlider(rect, label, minMaxAttribute, ref floatValue, ref maxFloat, false);

                    maxField.SetValue(context.Target, maxFloat);
                    return floatValue;
                }
            }

            var rangeAttribute = field.GetCustomAttribute<RangeAttribute>();
            if (rangeAttribute != null)
            {
                return EditorGUI.Slider(rect, label, floatValue, rangeAttribute.min, rangeAttribute.max);
            }

            floatValue = EditorGUI.FloatField(rect, label, floatValue);

            var minAttribute = field.GetCustomAttribute<MinAttribute>();
            if (minAttribute != null)
            {
                floatValue = Mathf.Max(minAttribute.Value, floatValue);
            }

            var maxAttribute = field.GetCustomAttribute<MaxAttribute>();
            if (maxAttribute != null)
            {
                floatValue = Mathf.Min(maxAttribute.Value, floatValue);
            }

            return floatValue;
        }

        public override float GetFieldsHeight(object target)
        {
            var context = InspectorContext.Current;
            var attribute = context?.Field?.GetCustomAttribute<MinMaxSliderAttribute>();
            if (attribute != null && !string.IsNullOrWhiteSpace(attribute.MaxFieldName))
            {
                return MinMaxSliderDrawer.GetHeight(attribute);
            }

            return base.GetFieldsHeight(target);
        }
    }
}
#endif
