#if UNITY_EDITOR
using System;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    public sealed class FloatFieldDrawerMatcher : FieldDrawerMatcher<UIToolkitFieldDrawer>
    {
        public override bool CanDraw(Type fieldType) => fieldType == typeof(float);
        public override Type DrawerType => typeof(FloatFieldDrawer);
    }

    public class FloatFieldDrawer : UIToolkitFieldDrawer
    {
        public override VisualElement CreateField(string label, Type fieldType, object value, Action<object> onValueChanged)
        {
            var field = new FloatField(label)
            {
                value = value != null ? (float)value : 0f
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
