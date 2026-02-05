#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class RangeFloatFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(FieldInfo field) =>
            field.FieldType == typeof(float) && field.GetCustomAttribute<RangeAttribute>() != null;
        public override IReadOnlyList<Type> AttributeTypes => new[] { typeof(RangeAttribute) };
        public override Type DrawerType => typeof(RangeFloatFieldDrawer);
    }

    public sealed class RangeFloatFieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            float floatValue = value != null ? (float)value : 0f;
            var rangeAttribute = field.GetCustomAttribute<RangeAttribute>();
            if (rangeAttribute == null)
            {
                return EditorGUI.FloatField(rect, label, floatValue);
            }

            return EditorGUI.Slider(rect, label, floatValue, rangeAttribute.min, rangeAttribute.max);
        }
    }
}
#endif

