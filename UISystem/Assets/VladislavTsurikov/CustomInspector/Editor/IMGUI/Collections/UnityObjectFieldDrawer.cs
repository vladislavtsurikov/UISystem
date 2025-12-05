#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public class UnityObjectFieldDrawer : IMGUIFieldDrawer
    {
        public override bool CanDraw(Type type) => typeof(Object).IsAssignableFrom(type);

        public override object Draw(Rect rect, GUIContent label, Type fieldType, object value) =>
            EditorGUI.ObjectField(rect, label, (Object)value, fieldType, true);

        public override bool ShouldCreateInstanceIfNull() => false;
    }
}
#endif
