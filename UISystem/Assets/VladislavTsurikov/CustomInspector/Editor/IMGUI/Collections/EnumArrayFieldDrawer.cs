#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class EnumArrayFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(FieldInfo field) =>
            field.FieldType.IsArray && field.FieldType.GetElementType()?.IsEnum == true;

        public override Type DrawerType => typeof(EnumArrayFieldDrawer);
    }

    public class EnumArrayFieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            Type enumType = field.FieldType.GetElementType();
            Array enumArray = (Array)value ?? Array.CreateInstance(enumType, 0);

            var mask = 0;
            foreach (Enum enumValue in enumArray)
            {
                mask |= 1 << Convert.ToInt32(enumValue);
            }

            EditorGUI.BeginChangeCheck();
            mask = EditorGUI.MaskField(rect, label, mask, Enum.GetNames(enumType));

            if (EditorGUI.EndChangeCheck())
            {
                var selectedValues = new List<Enum>();

                Array enumValues = Enum.GetValues(enumType);
                for (var i = 0; i < enumValues.Length; i++)
                {
                    if ((mask & (1 << i)) != 0)
                    {
                        selectedValues.Add((Enum)enumValues.GetValue(i));
                    }
                }

                var newArray = Array.CreateInstance(enumType, selectedValues.Count);
                for (var i = 0; i < selectedValues.Count; i++)
                {
                    newArray.SetValue(selectedValues[i], i);
                }

                return newArray;
            }

            return enumArray;
        }
    }
}
#endif


