#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public class EnumFieldDrawer : IMGUIFieldDrawer
    {
        public override bool CanDraw(Type fieldType) => fieldType.BaseType == typeof(Enum);

        public override object Draw(Rect rect, GUIContent label, Type fieldType, object value) =>
            EditorGUI.EnumPopup(rect, label, (Enum)value);
    }
}
#endif
