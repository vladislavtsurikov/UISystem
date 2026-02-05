#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit.Decorators
{
    public sealed class HelpBoxDecoratorDrawerMatcher : DecoratorDrawerMatcher<UIToolkitDecoratorDrawer>
    {
        public override bool CanDraw(Attribute attribute) => attribute is HelpBoxAttribute;
        public override Type DrawerType => typeof(HelpBoxDecoratorDrawer);
    }

    public sealed class HelpBoxDecoratorDrawer : UIToolkitDecoratorDrawer
    {
        public override VisualElement CreateElement(FieldInfo field, object target)
        {
            if (Attribute is not HelpBoxAttribute helpBoxAttribute)
            {
                return new VisualElement();
            }

            var helpBox = new HelpBox(helpBoxAttribute.Message, GetMessageType(helpBoxAttribute.MessageType));
            helpBox.style.marginTop = 2;
            helpBox.style.marginBottom = 2;

            return helpBox;
        }

        private static HelpBoxMessageType GetMessageType(HelpBoxMessageType messageType)
        {
            return messageType switch
            {
                HelpBoxMessageType.Warning => HelpBoxMessageType.Warning,
                HelpBoxMessageType.Error => HelpBoxMessageType.Error,
                HelpBoxMessageType.Info => HelpBoxMessageType.Info,
                _ => HelpBoxMessageType.None
            };
        }
    }
}
#endif

