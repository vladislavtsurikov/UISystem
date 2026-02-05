#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class RangeIntFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(FieldInfo field) =>
            field.FieldType == typeof(int) && field.GetCustomAttribute<RangeAttribute>() != null;
        public override IReadOnlyList<Type> AttributeTypes => new[] { typeof(RangeAttribute) };
        public override Type DrawerType => typeof(RangeIntFieldDrawer);
    }

    public sealed class RangeIntFieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            int intValue = value != null ? (int)value : 0;
            var rangeAttribute = field.GetCustomAttribute<RangeAttribute>();
            if (rangeAttribute == null)
            {
                return EditorGUI.IntField(rect, label, intValue);
            }

            return EditorGUI.IntSlider(rect, label, intValue, (int)rangeAttribute.min, (int)rangeAttribute.max);
        }
    }
}
#endif

