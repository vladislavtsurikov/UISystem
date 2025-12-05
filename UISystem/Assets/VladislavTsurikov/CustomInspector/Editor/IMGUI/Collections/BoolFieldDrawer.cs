#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public class BoolFieldDrawer : IMGUIFieldDrawer
    {
        public override bool CanDraw(Type type) => type == typeof(bool);

        public override object Draw(Rect rect, GUIContent label, Type fieldType, object value) =>
            EditorGUI.Toggle(rect, label, value != null && (bool)value);
    }
}
#endif
