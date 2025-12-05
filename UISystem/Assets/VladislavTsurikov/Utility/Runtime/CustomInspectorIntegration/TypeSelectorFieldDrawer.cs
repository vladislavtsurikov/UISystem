#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;
using VladislavTsurikov.Utility.Runtime.CustomInspectorIntegration;

namespace VladislavTsurikov.ReflectionUtility.Runtime.CustomInspectorIntegration
{
    public class TypeSelectorFieldDrawer : IMGUIFieldDrawer
    {
        public override bool CanDraw(Type type) =>
            type.IsGenericType && type.GetGenericTypeDefinition() == typeof(TypeSelector<>);

        public override object Draw(Rect rect, GUIContent label, Type fieldType, object value)
        {
            if (value == null)
            {
                return null;
            }

            Type baseType = fieldType.GenericTypeArguments[0];

            dynamic typeReference = Convert.ChangeType(value, fieldType);

            if (typeReference == null)
            {
                return value;
            }

            var labelRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            var buttonRect = new Rect(rect.x + EditorGUIUtility.labelWidth + 5, rect.y,
                rect.width - EditorGUIUtility.labelWidth - 5, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(labelRect, label);

            string currentTypeName = typeReference.Type != null ? typeReference.Type.Name : "(None)";
            if (GUI.Button(buttonRect, currentTypeName))
            {
                TypeSelectorPopup.Open(
                    buttonRect,
                    $"Select {baseType.Name}",
                    GetDerivedTypes(baseType),
                    type => type.Name,
                    selectedType => { typeReference.Type = selectedType; });
            }

            return value;
        }

        private static IEnumerable<Type> GetDerivedTypes(Type baseType)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            foreach (Type type in assembly.GetTypes())
            {
                if (baseType.IsAssignableFrom(type) && !type.IsAbstract)
                {
                    yield return type;
                }
            }
        }
    }
}
#endif
