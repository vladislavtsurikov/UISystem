#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class InfoBoxDecoratorDrawerMatcher : DecoratorDrawerMatcher<IMGUIDecoratorDrawer>
    {
        public override bool CanDraw(Attribute attribute) => attribute is InfoBoxAttribute;

        public override Type DrawerType => typeof(InfoBoxDecoratorDrawer);
    }

    public class InfoBoxDecoratorDrawer : IMGUIDecoratorDrawer
    {
        private InfoBoxAttribute _attribute;

        public override void Initialize(Attribute attribute)
        {
            _attribute = attribute as InfoBoxAttribute;
        }

        public override void Draw(Rect rect, FieldInfo field, object target)
        {
            if (target == null || !_attribute.IsVisible(target))
            {
                return;
            }

            string message = _attribute.GetMessage(target);

            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            MessageType messageType = ConvertMessageType(_attribute.MessageType);
            EditorGUI.HelpBox(rect, message, messageType);
        }

        public override float GetHeight(FieldInfo field, object target)
        {
            if (target == null || !_attribute.IsVisible(target))
            {
                return 0;
            }

            string message = _attribute.GetMessage(target);

            if (string.IsNullOrWhiteSpace(message))
            {
                return 0;
            }

            var content = new GUIContent(message);
            var style = EditorStyles.helpBox;
            float height = style.CalcHeight(content, EditorGUIUtility.currentViewWidth - 12);

            return height + 6f;
        }

        private MessageType ConvertMessageType(InfoBoxMessageType type)
        {
            return type switch
            {
                InfoBoxMessageType.None => MessageType.None,
                InfoBoxMessageType.Info => MessageType.Info,
                InfoBoxMessageType.Warning => MessageType.Warning,
                InfoBoxMessageType.Error => MessageType.Error,
                _ => MessageType.None
            };
        }
    }
}
#endif
