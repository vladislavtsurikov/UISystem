#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class UnityObjectFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(Type fieldType) => typeof(Object).IsAssignableFrom(fieldType);
        public override Type DrawerType => typeof(UnityObjectFieldDrawer);
    }

    public class UnityObjectFieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object value) =>
            EditorGUI.ObjectField(rect, label, (Object)value, field.FieldType, true);

        public override bool ShouldCreateInstanceIfNull() => false;
    }
}
#endif
