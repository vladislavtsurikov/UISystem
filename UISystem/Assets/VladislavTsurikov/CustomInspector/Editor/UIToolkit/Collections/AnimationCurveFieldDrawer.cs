#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    public sealed class AnimationCurveFieldDrawerMatcher : FieldDrawerMatcher<UIToolkitFieldDrawer>
    {
        public override bool CanDraw(Type fieldType) => fieldType == typeof(AnimationCurve);
        public override Type DrawerType => typeof(AnimationCurveFieldDrawer);
    }

    public class AnimationCurveFieldDrawer : UIToolkitFieldDrawer
    {
        public override VisualElement CreateField(string label, Type fieldType, object value, Action<object> onValueChanged)
        {
            var field = new CurveField(label)
            {
                value = value as AnimationCurve ?? new AnimationCurve()
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
