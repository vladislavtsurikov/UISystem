#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;
using System.Reflection;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class FloatFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(FieldInfo field) => field.FieldType == typeof(float);
        public override Type DrawerType => typeof(FloatFieldDrawer);
    }

    public class FloatFieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            float floatValue = value != null ? (float)value : 0f;
            return EditorGUI.FloatField(rect, label, floatValue);
        }
    }
}
#endif


