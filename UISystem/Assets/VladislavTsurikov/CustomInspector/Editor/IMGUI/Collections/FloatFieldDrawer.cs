#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class FloatFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(Type fieldType) => fieldType == typeof(float);
        public override Type DrawerType => typeof(FloatFieldDrawer);
    }

    public class FloatFieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, Type fieldType, object value) =>
            EditorGUI.FloatField(rect, label, value != null ? (float)value : 0f);
    }
}
#endif
