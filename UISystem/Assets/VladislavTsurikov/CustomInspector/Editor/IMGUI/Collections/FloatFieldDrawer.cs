#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public class FloatFieldDrawer : IMGUIFieldDrawer
    {
        public override bool CanDraw(Type type) => type == typeof(float);

        public override object Draw(Rect rect, GUIContent label, Type fieldType, object value) =>
            EditorGUI.FloatField(rect, label, value != null ? (float)value : 0f);
    }
}
#endif
