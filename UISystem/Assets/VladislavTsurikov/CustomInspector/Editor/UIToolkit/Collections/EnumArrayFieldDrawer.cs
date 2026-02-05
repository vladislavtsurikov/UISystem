#if UNITY_EDITOR
using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    public sealed class EnumArrayFieldDrawerMatcher : FieldDrawerMatcher<UIToolkitFieldDrawer>
    {
        public override bool CanDraw(FieldInfo field) =>
            field.FieldType.IsArray && field.FieldType.GetElementType()?.IsEnum == true;

        public override Type DrawerType => typeof(EnumArrayFieldDrawer);
    }

    public class EnumArrayFieldDrawer : UIToolkitFieldDrawer
    {
        public override VisualElement CreateField(string label, Type fieldType, object value, Action<object> onValueChanged)
        {
            Type enumType = fieldType.GetElementType();
            Array enumArray = (Array)value ?? Array.CreateInstance(enumType, 0);

            var mask = 0;
            foreach (Enum enumValue in enumArray)
            {
                mask |= 1 << Convert.ToInt32(enumValue);
            }

            var defaultEnumValue = (Enum)Activator.CreateInstance(enumType);
            var field = new EnumFlagsField(label, defaultEnumValue);
            field.value = defaultEnumValue;

            var enumValues = Enum.GetValues(enumType);
            int initialMask = 0;
            foreach (Enum ev in enumArray)
            {
                for (int i = 0; i < enumValues.Length; i++)
                {
                    if (ev.Equals(enumValues.GetValue(i)))
                    {
                        initialMask |= 1 << i;
                    }
                }
            }

            field.RegisterValueChangedCallback(evt =>
            {
                var selectedValues = new List<Enum>();
                var newMask = Convert.ToInt32(evt.newValue);

                for (var i = 0; i < enumValues.Length; i++)
                {
                    if ((newMask & (1 << i)) != 0)
                    {
                        selectedValues.Add((Enum)enumValues.GetValue(i));
                    }
                }

                var newArray = Array.CreateInstance(enumType, selectedValues.Count);
                for (var i = 0; i < selectedValues.Count; i++)
                {
                    newArray.SetValue(selectedValues[i], i);
                }

                onValueChanged?.Invoke(newArray);
            });

            return field;
        }
    }
}
#endif



