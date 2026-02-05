#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class Vector2FieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(FieldInfo field) => field.FieldType == typeof(Vector2);
        public override Type DrawerType => typeof(Vector2FieldDrawer);
    }

    public class Vector2FieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            var vectorValue = value != null ? (Vector2)value : Vector2.zero;
            return EditorGUI.Vector2Field(rect, label, vectorValue);
        }
    }
}
#endif


