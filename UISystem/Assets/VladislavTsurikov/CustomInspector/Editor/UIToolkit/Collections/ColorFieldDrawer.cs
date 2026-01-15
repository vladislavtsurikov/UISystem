#if UNITY_EDITOR
using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    public sealed class ColorFieldDrawerMatcher : FieldDrawerMatcher<UIToolkitFieldDrawer>
    {
        public override bool CanDraw(Type fieldType) => fieldType == typeof(Color);
        public override Type DrawerType => typeof(ColorFieldDrawer);
    }

    public class ColorFieldDrawer : UIToolkitFieldDrawer
    {
        public override VisualElement CreateField(string label, Type fieldType, object value, Action<object> onValueChanged)
        {
            var field = new ColorField(label)
            {
                value = value != null ? (Color)value : Color.white
            };

            field.RegisterValueChangedCallback(evt =>
            {
                onValueChanged?.Invoke(evt.newValue);
            });

            return field;
        }
    }
}
#endif
