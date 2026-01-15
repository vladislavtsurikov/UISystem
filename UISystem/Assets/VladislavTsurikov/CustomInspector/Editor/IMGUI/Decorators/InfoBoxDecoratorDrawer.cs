#if UNITY_EDITOR
using System;
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

        public override void Draw(Rect rect)
        {
            var context = InspectorContext.Current;
            if (context == null)
            {
                return;
            }

            if (!_attribute.IsVisible(context.Target))
            {
                return;
            }

            string message = _attribute.GetMessage(context.Target);

            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            MessageType messageType = ConvertMessageType(_attribute.MessageType);
            EditorGUI.HelpBox(rect, message, messageType);
        }

        public override float GetHeight()
        {
            var context = InspectorContext.Current;
            if (context == null)
            {
                return 0;
            }

            if (!_attribute.IsVisible(context.Target))
            {
                return 0;
            }

            string message = _attribute.GetMessage(context.Target);

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
