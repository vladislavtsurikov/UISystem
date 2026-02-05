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
    public sealed class MinMaxSliderVector2FieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(FieldInfo field) =>
            field.FieldType == typeof(Vector2) && field.GetCustomAttribute<MinMaxSliderAttribute>() != null;
        public override IReadOnlyList<Type> AttributeTypes => new[] { typeof(MinMaxSliderAttribute) };
        public override Type DrawerType => typeof(MinMaxSliderVector2FieldDrawer);
    }

    public sealed class MinMaxSliderVector2FieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            var vectorValue = value != null ? (Vector2)value : Vector2.zero;
            var minMaxAttribute = field.GetCustomAttribute<MinMaxSliderAttribute>();
            if (minMaxAttribute == null)
            {
                return EditorGUI.Vector2Field(rect, label, vectorValue);
            }

            float minValue = vectorValue.x;
            float maxValue = vectorValue.y;

            MinMaxSliderDrawer.DrawSlider(
                rect,
                label,
                minMaxAttribute,
                ref minValue,
                ref maxValue,
                false,
                target);

            return new Vector2(minValue, maxValue);
        }

        public override float GetFieldsHeight(object target, FieldInfo field, object value)
        {
            var attribute = field?.GetCustomAttribute<MinMaxSliderAttribute>();
            if (attribute != null)
            {
                return MinMaxSliderDrawer.GetHeight(attribute);
            }

            return base.GetFieldsHeight(target, field, value);
        }
    }
}
#endif






