#if UNITY_EDITOR
using System;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    public sealed class DoubleFieldDrawerMatcher : FieldDrawerMatcher<UIToolkitFieldDrawer>
    {
        public override bool CanDraw(Type fieldType) => fieldType == typeof(double);
        public override Type DrawerType => typeof(DoubleFieldDrawer);
    }

    public class DoubleFieldDrawer : UIToolkitFieldDrawer
    {
        public override VisualElement CreateField(string label, Type fieldType, object value, Action<object> onValueChanged)
        {
            var field = new DoubleField(label)
            {
                value = value != null ? (double)value : 0.0
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
