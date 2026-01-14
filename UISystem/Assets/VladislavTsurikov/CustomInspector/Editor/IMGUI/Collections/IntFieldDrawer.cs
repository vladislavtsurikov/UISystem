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
    public sealed class IntFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(Type fieldType) => fieldType == typeof(int);
        public override Type DrawerType => typeof(IntFieldDrawer);
    }

    public class IntFieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object value)
        {
            int intValue = value != null ? (int)value : 0;

            var minMaxAttribute = field.GetCustomAttribute<MinMaxSliderAttribute>();
            if (minMaxAttribute != null && !string.IsNullOrWhiteSpace(minMaxAttribute.MaxFieldName))
            {
                var context = InspectorContext.Current;
                if (MinMaxSliderDrawer.TryGetPairedField(context?.Target, minMaxAttribute.MaxFieldName,
                        out var maxField))
                {
                    var maxValue = maxField.GetValue(context.Target);
                    var maxFloat = maxValue is int intMax ? intMax : intValue;
                    float minFloat = intValue;

                    MinMaxSliderDrawer.DrawSlider(rect, label, minMaxAttribute, ref minFloat, ref maxFloat, true);

                    intValue = Mathf.RoundToInt(minFloat);
                    maxField.SetValue(context.Target, Mathf.RoundToInt(maxFloat));
                    return intValue;
                }
            }

            var rangeAttribute = field.GetCustomAttribute<RangeAttribute>();
            if (rangeAttribute != null)
            {
                return EditorGUI.IntSlider(rect, label, intValue, (int)rangeAttribute.min, (int)rangeAttribute.max);
            }

            intValue = EditorGUI.IntField(rect, label, intValue);

            var minAttribute = field.GetCustomAttribute<MinAttribute>();
            if (minAttribute != null)
            {
                intValue = Mathf.Max((int)minAttribute.Value, intValue);
            }

            var maxAttribute = field.GetCustomAttribute<MaxAttribute>();
            if (maxAttribute != null)
            {
                intValue = Mathf.Min((int)maxAttribute.Value, intValue);
            }

            return intValue;
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
