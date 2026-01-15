#if UNITY_EDITOR
using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit.Decorators
{
    public sealed class RequiredDecoratorDrawerMatcher : DecoratorDrawerMatcher<UIToolkitDecoratorDrawer>
    {
        public override bool CanDraw(Attribute attribute) => attribute is RequiredAttribute;
        public override Type DrawerType => typeof(RequiredDecoratorDrawer);
    }

    public sealed class RequiredDecoratorDrawer : UIToolkitDecoratorDrawer
    {
        private RequiredAttribute _attribute;
        private HelpBox _helpBox;
        private FieldInfo _field;
        private object _target;

        public override void Initialize(Attribute attribute)
        {
            base.Initialize(attribute);
            _attribute = attribute as RequiredAttribute;
        }

        public override VisualElement CreateElement()
        {
            if (_attribute == null)
            {
                return new VisualElement();
            }

            _helpBox = new HelpBox(_attribute.Message, HelpBoxMessageType.Error);
            _helpBox.style.marginTop = 2;
            _helpBox.style.marginBottom = 2;

            UpdateVisibility();

            return _helpBox;
        }

        private void UpdateVisibility()
        {
            var context = InspectorContext.Current;
            if (context == null)
            {
                _helpBox.style.display = DisplayStyle.None;
                return;
            }

            _field = context.Field;
            _target = context.Target;

            if (IsFieldValid())
            {
                _helpBox.style.display = DisplayStyle.None;
            }
            else
            {
                _helpBox.style.display = DisplayStyle.Flex;
            }
        }

        private bool IsFieldValid()
        {
            if (_field == null || _target == null)
            {
                return true;
            }

            object value = _field.GetValue(_target);

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
