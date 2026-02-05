#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;
using System.Reflection;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class IntFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(FieldInfo field) => field.FieldType == typeof(int);
        public override Type DrawerType => typeof(IntFieldDrawer);
    }

    public class IntFieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            int intValue = value != null ? (int)value : 0;

            return EditorGUI.IntField(rect, label, intValue);
        }
    }
}
#endif


