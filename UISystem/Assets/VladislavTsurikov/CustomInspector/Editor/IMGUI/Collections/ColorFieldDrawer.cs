#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class ColorFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(Type fieldType) => fieldType == typeof(Color);
        public override Type DrawerType => typeof(ColorFieldDrawer);
    }

    public class ColorFieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object value)
        {
            Color colorValue = value != null ? (Color)value : default(Color);
            return EditorGUI.ColorField(rect, label, colorValue);
        }
    }
}
#endif
