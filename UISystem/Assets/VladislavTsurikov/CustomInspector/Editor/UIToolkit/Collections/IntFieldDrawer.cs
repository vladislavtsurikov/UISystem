#if UNITY_EDITOR
using System;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    public sealed class IntFieldDrawerMatcher : FieldDrawerMatcher<UIToolkitFieldDrawer>
    {
        public override bool CanDraw(Type fieldType) => fieldType == typeof(int);
        public override Type DrawerType => typeof(IntFieldDrawer);
    }

    public class IntFieldDrawer : UIToolkitFieldDrawer
    {
        public override VisualElement CreateField(string label, Type fieldType, object value, Action<object> onValueChanged)
        {
            var field = new IntegerField(label)
            {
                value = value != null ? (int)value : 0
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
