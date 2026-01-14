#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class Vector2FieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(Type fieldType) => fieldType == typeof(Vector2);
        public override Type DrawerType => typeof(Vector2FieldDrawer);
    }

    public class Vector2FieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object value)
        {
            var vectorValue = value != null ? (Vector2)value : Vector2.zero;
            var minMaxAttribute = field.GetCustomAttribute<MinMaxSliderAttribute>();
            if (minMaxAttribute != null)
            {
                float minValue = vectorValue.x;
                float maxValue = vectorValue.y;

                MinMaxSliderDrawer.DrawSlider(rect, label, minMaxAttribute, ref minValue, ref maxValue, false);

                return new Vector2(minValue, maxValue);
            }

            return EditorGUI.Vector2Field(rect, label, vectorValue);
        }

        public override float GetFieldsHeight(object target)
        {
            var context = InspectorContext.Current;
            var attribute = context?.Field?.GetCustomAttribute<MinMaxSliderAttribute>();
            if (attribute != null)
            {
                return MinMaxSliderDrawer.GetHeight(attribute);
            }

            return base.GetFieldsHeight(target);
        }
    }
}
#endif
