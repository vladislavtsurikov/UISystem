#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class GradientFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(FieldInfo field) => field.FieldType == typeof(Gradient);
        public override Type DrawerType => typeof(GradientFieldDrawer);
    }

    public class GradientFieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            var gradient = value as Gradient ?? new Gradient();
            return EditorGUI.GradientField(rect, label, gradient);
        }

        public override bool ShouldCreateInstanceIfNull() => false;
    }
}
#endif


