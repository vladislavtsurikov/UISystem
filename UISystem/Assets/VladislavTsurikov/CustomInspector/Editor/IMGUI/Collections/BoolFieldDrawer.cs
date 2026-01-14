#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class BoolFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(Type fieldType) => fieldType == typeof(bool);
        public override Type DrawerType => typeof(BoolFieldDrawer);
    }

    public class BoolFieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object value) =>
            EditorGUI.Toggle(rect, label, value != null && (bool)value);
    }
}
#endif
