#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit.Decorators
{
    public sealed class PreviewTextureDecoratorDrawerMatcher : DecoratorDrawerMatcher<UIToolkitDecoratorDrawer>
    {
        public override bool CanDraw(Attribute attribute) => attribute is PreviewTextureAttribute;
        public override Type DrawerType => typeof(PreviewTextureDecoratorDrawer);
    }

    public sealed class PreviewTextureDecoratorDrawer : UIToolkitDecoratorDrawer
    {
        private PreviewTextureAttribute _attribute;
        private Image _imageElement;

        public override void Initialize(Attribute attribute)
        {
            base.Initialize(attribute);
            _attribute = attribute as PreviewTextureAttribute;
        }

        public override VisualElement CreateElement(FieldInfo field, object target)
        {
            if (_attribute == null)
            {
                return new VisualElement();
            }

            _imageElement = new Image();
            _imageElement.style.height = _attribute.Height;

            if (_attribute.Width > 0)
            {
                _imageElement.style.width = _attribute.Width;
            }

            _imageElement.scaleMode = ScaleMode.ScaleToFit;
            _imageElement.style.marginTop = 2;
            _imageElement.style.marginBottom = 2;

            UpdateImage(field, target);

            return _imageElement;
        }

        private void UpdateImage(FieldInfo field, object target)
        {
            if (field == null || target == null)
            {
                _imageElement.style.display = DisplayStyle.None;
                return;
            }

            var texture = field.GetValue(target) as Texture;
            if (texture == null)
            {
                _imageElement.style.display = DisplayStyle.None;
                return;
            }

            _imageElement.style.display = DisplayStyle.Flex;
            _imageElement.image = texture;
        }
    }
}
#endif

