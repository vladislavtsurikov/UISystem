#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI.Decorators
{
    public sealed class HelpBoxDecoratorDrawerMatcher : DecoratorDrawerMatcher<IMGUIDecoratorDrawer>
    {
        public override bool CanDraw(Attribute attribute) => attribute is HelpBoxAttribute;
        public override Type DrawerType => typeof(HelpBoxDecoratorDrawer);
    }

    public sealed class HelpBoxDecoratorDrawer : IMGUIDecoratorDrawer
    {
        private GUIContent _content;
        private MessageType _messageType;

        public override void Draw(Rect rect, FieldInfo field, object target)
        {
            if (Attribute is HelpBoxAttribute helpBoxAttribute)
            {
                _content = new GUIContent(helpBoxAttribute.Message);
                _messageType = GetMessageType(helpBoxAttribute.MessageType);
            }

            EditorGUI.HelpBox(rect, _content.text, _messageType);
        }

        public override float GetHeight(FieldInfo field, object target)
        {
            if (Attribute is not HelpBoxAttribute helpBoxAttribute)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            var style = EditorStyles.helpBox;
            return Mathf.Max(EditorGUIUtility.singleLineHeight,
                style.CalcHeight(new GUIContent(helpBoxAttribute.Message), EditorGUIUtility.currentViewWidth));
        }

        private static MessageType GetMessageType(HelpBoxMessageType messageType)
        {
            return messageType switch
            {
                HelpBoxMessageType.Warning => MessageType.Warning,
                HelpBoxMessageType.Error => MessageType.Error,
                HelpBoxMessageType.Info => MessageType.Info,
                _ => MessageType.None
            };
        }
    }
}
#endif
