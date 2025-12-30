#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class Vector3FieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(Type fieldType) => fieldType == typeof(Vector3);
        public override Type DrawerType => typeof(Vector3FieldDrawer);
    }

    public class Vector3FieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, Type fieldType, object value) =>
            EditorGUI.Vector3Field(rect, label, (Vector3)value);
    }
}
#endif
