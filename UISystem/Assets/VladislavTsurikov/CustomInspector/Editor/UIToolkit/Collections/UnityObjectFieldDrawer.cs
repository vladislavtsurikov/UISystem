#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    public sealed class UnityObjectFieldDrawerMatcher : FieldDrawerMatcher<UIToolkitFieldDrawer>
    {
        public override bool CanDraw(FieldInfo field) => typeof(UnityEngine.Object).IsAssignableFrom(field.FieldType);
        public override Type DrawerType => typeof(UnityObjectFieldDrawer);
    }

    public class UnityObjectFieldDrawer : UIToolkitFieldDrawer
    {
        public override VisualElement CreateField(string label, Type fieldType, object value, Action<object> onValueChanged)
        {
            var field = new ObjectField(label)
            {
                objectType = fieldType,
                value = value as UnityEngine.Object
            };

            field.RegisterValueChangedCallback(evt =>
            {
                onValueChanged?.Invoke(evt.newValue);
            });

            return field;
        }

        public override bool ShouldCreateInstanceIfNull() => false;
    }
}
#endif



