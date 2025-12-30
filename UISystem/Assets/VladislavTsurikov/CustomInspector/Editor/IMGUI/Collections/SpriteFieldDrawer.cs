#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class SpriteFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(Type fieldType) => fieldType == typeof(Sprite);
        public override Type DrawerType => typeof(SpriteFieldDrawer);
    }

    public class SpriteFieldDrawer : IMGUIFieldDrawer
    {
        private const float ObjectFieldSize = 50f;

        public override object Draw(Rect rect, GUIContent label, Type fieldType, object value)
        {
            var labelWidth = EditorGUIUtility.labelWidth;
            var labelHeight = EditorGUIUtility.singleLineHeight;
            var labelRect = new Rect(rect.x, rect.y, labelWidth, labelHeight);
            var fieldRect = new Rect(rect.x + labelWidth, rect.y, ObjectFieldSize, ObjectFieldSize);

            EditorGUI.LabelField(labelRect, label);

            return EditorGUI.ObjectField(fieldRect, (Sprite)value, typeof(Sprite), true);
        }

        public override bool ShouldCreateInstanceIfNull() => false;

        public override float GetFieldsHeight(object target) => 50;
    }
}
#endif
