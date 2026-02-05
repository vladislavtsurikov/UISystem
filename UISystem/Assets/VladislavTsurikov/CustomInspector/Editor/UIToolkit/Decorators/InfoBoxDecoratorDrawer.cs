#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit.Decorators
{
    public sealed class InfoBoxDecoratorDrawerMatcher : DecoratorDrawerMatcher<UIToolkitDecoratorDrawer>
    {
        public override bool CanDraw(Attribute attribute) => attribute is InfoBoxAttribute;
        public override Type DrawerType => typeof(InfoBoxDecoratorDrawer);
    }

    public sealed class InfoBoxDecoratorDrawer : UIToolkitDecoratorDrawer
    {
        private InfoBoxAttribute _attribute;
        private HelpBox _helpBox;

        public override void Initialize(Attribute attribute)
        {
            base.Initialize(attribute);
            _attribute = attribute as InfoBoxAttribute;
        }

        public override VisualElement CreateElement(FieldInfo field, object target)
        {
            if (_attribute == null)
            {
                return new VisualElement();
            }

            _helpBox = new HelpBox();
            _helpBox.style.marginTop = 2;
            _helpBox.style.marginBottom = 2;

            UpdateHelpBox(target);

            return _helpBox;
        }

        private void UpdateHelpBox(object target)
        {
            if (target == null)
            {
                _helpBox.style.display = DisplayStyle.None;
                return;
            }

            if (!_attribute.IsVisible(target))
            {
                _helpBox.style.display = DisplayStyle.None;
                return;
            }

            string message = _attribute.GetMessage(target);

            if (string.IsNullOrWhiteSpace(message))
            {
                _helpBox.style.display = DisplayStyle.None;
                return;
            }

            _helpBox.style.display = DisplayStyle.Flex;
            _helpBox.text = message;
            _helpBox.messageType = ConvertMessageType(_attribute.MessageType);
        }

        private static HelpBoxMessageType ConvertMessageType(InfoBoxMessageType type)
        {
            return type switch
            {
                InfoBoxMessageType.None => HelpBoxMessageType.None,
                InfoBoxMessageType.Info => HelpBoxMessageType.Info,
                InfoBoxMessageType.Warning => HelpBoxMessageType.Warning,
                InfoBoxMessageType.Error => HelpBoxMessageType.Error,
                _ => HelpBoxMessageType.None
            };
        }
    }
}
#endif

