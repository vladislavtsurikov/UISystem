#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class Vector3FieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(FieldInfo field) => field.FieldType == typeof(Vector3);
        public override Type DrawerType => typeof(Vector3FieldDrawer);
    }

    public class Vector3FieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            var vectorValue = value != null ? (Vector3)value : Vector3.zero;
            return EditorGUI.Vector3Field(rect, label, vectorValue);
        }
    }
}
#endif


