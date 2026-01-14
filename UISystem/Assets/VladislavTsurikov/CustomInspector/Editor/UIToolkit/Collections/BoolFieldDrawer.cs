#if UNITY_EDITOR
using System;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    public sealed class BoolFieldDrawerMatcher : FieldDrawerMatcher<UIToolkitFieldDrawer>
    {
        public override bool CanDraw(Type fieldType) => fieldType == typeof(bool);
        public override Type DrawerType => typeof(BoolFieldDrawer);
    }

    public class BoolFieldDrawer : UIToolkitFieldDrawer
    {
        public override VisualElement CreateField(string label, Type fieldType, object value, Action<object> onValueChanged)
        {
            var field = new Toggle(label)
            {
                value = value != null && (bool)value
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
