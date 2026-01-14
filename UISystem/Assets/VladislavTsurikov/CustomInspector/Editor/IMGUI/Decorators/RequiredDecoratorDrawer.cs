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
        private FieldInfo _field;
        private object _target;

        public override void OnSetAttribute(Attribute attribute)
        {
            _attribute = attribute as RequiredAttribute;
        }

        public override void Draw(Rect rect)
        {
            var context = InspectorContext.Current;
            if (context == null)
            {
                return;
            }

            _field = context.Field;
            _target = context.Target;

            if (IsFieldValid())
            {
                return;
            }

            // Draw error help box
            var style = EditorStyles.helpBox;
            var content = new GUIContent(_attribute.Message, EditorGUIUtility.IconContent("console.erroricon.sml").image);

            EditorGUI.HelpBox(rect, content.text, MessageType.Error);
        }

        public override float GetHeight()
        {
            var context = InspectorContext.Current;
            if (context == null || IsFieldValid())
            {
                return 0;
            }

            // Calculate height for error message
            var content = new GUIContent(_attribute.Message);
            var style = EditorStyles.helpBox;
            return style.CalcHeight(content, EditorGUIUtility.currentViewWidth) + 4f;
        }

        private bool IsFieldValid()
        {
            if (_field == null || _target == null)
            {
                return true;
            }

            object value = _field.GetValue(_target);

            // Check for null
            if (value == null)
            {
                return false;
            }

            // Check for UnityEngine.Object (handles destroyed objects)
            if (value is UnityEngine.Object unityObject)
            {
                return unityObject != null;
            }

            // Check for string
            if (value is string stringValue)
            {
                return !string.IsNullOrWhiteSpace(stringValue);
            }

            // Check for collections
            if (value is ICollection collection)
            {
                return collection.Count > 0;
            }

            return true;
        }
    }
}
#endif
