#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    public sealed class SpriteFieldDrawerMatcher : FieldDrawerMatcher<UIToolkitFieldDrawer>
    {
        public override bool CanDraw(FieldInfo field) => field.FieldType == typeof(Sprite);
        public override Type DrawerType => typeof(SpriteFieldDrawer);
    }

    public class SpriteFieldDrawer : UIToolkitFieldDrawer
    {
        private const float ObjectFieldSize = 64f;

        public override VisualElement CreateField(string label, Type fieldType, object value, Action<object> onValueChanged)
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;

            var labelElement = new Label(label);
            labelElement.style.minWidth = 120;

            var field = new ObjectField
            {
                objectType = typeof(Sprite),
                value = value as Sprite
            };

            field.style.width = ObjectFieldSize;
            field.style.height = ObjectFieldSize;

            field.RegisterValueChangedCallback(evt =>
            {
                onValueChanged?.Invoke(evt.newValue);
            });

            container.Add(labelElement);
            container.Add(field);

            return container;
        }

        public override bool ShouldCreateInstanceIfNull() => false;
    }
}
#endif



