#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    public sealed class LayerMaskFieldDrawerMatcher : FieldDrawerMatcher<UIToolkitFieldDrawer>
    {
        public override bool CanDraw(FieldInfo field) => field.FieldType == typeof(LayerMask);
        public override Type DrawerType => typeof(LayerMaskFieldDrawer);
    }

    public class LayerMaskFieldDrawer : UIToolkitFieldDrawer
    {
        public override VisualElement CreateField(string label, Type fieldType, object value, Action<object> onValueChanged)
        {
            var layerMask = value != null ? (LayerMask)value : new LayerMask();
            var field = new LayerMaskField(label, layerMask.value);

            field.RegisterValueChangedCallback(evt =>
            {
                LayerMask newLayerMask = evt.newValue;
                onValueChanged?.Invoke(newLayerMask);
            });

            return field;
        }
    }
}
#endif



