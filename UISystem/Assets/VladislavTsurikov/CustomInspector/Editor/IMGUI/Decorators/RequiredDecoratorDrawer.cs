#if UNITY_EDITOR
using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class RequiredDecoratorDrawerMatcher : DecoratorDrawerMatcher<IMGUIDecoratorDrawer>
    {
        public override bool CanDraw(Attribute attribute) => attribute is RequiredAttribute;
        public override Type DrawerType => typeof(RequiredDecoratorDrawer);
    }

    public class RequiredDecoratorDrawer : IMGUIDecoratorDrawer
    {
        private RequiredAttribute _attribute;

        public override void Initialize(Attribute attribute)
        {
            _attribute = attribute as RequiredAttribute;
        }

        public override void Draw(Rect rect, FieldInfo field, object target)
        {
            if (IsFieldValid(field, target))
            {
                return;
            }

            var style = EditorStyles.helpBox;
            var content = new GUIContent(_attribute.Message, EditorGUIUtility.IconContent("console.erroricon.sml").image);

            EditorGUI.HelpBox(rect, content.text, MessageType.Error);
        }

        public override float GetHeight(FieldInfo field, object target)
        {
            if (IsFieldValid(field, target))
            {
                return 0;
            }

            var content = new GUIContent(_attribute.Message);
            var style = EditorStyles.helpBox;
            return style.CalcHeight(content, EditorGUIUtility.currentViewWidth) + 4f;
        }

        private bool IsFieldValid(FieldInfo field, object target)
        {
            if (field == null || target == null)
            {
                return true;
            }

            object value = field.GetValue(target);

            if (value == null)
            {
                return false;
            }

            if (value is UnityEngine.Object unityObject)
            {
                return unityObject != null;
            }

            if (value is string stringValue)
            {
                return !string.IsNullOrWhiteSpace(stringValue);
            }

            if (value is ICollection collection)
            {
                return collection.Count > 0;
            }

            return true;
        }
    }
}
#endif
