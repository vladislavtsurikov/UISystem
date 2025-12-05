#if UNITY_EDITOR
using System;
using Assemblies.VladislavTsurikov.ComponentStack.Runtime.SingleElementStack;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack.SingleElementStackEditor.CustomInspectorIntegration
{
    public class SingleElementStackFieldDrawer : IMGUIFieldDrawer
    {
        public override bool CanDraw(Type type) =>
            type.IsGenericType && type.GetGenericTypeDefinition() == typeof(SingleElementStack<>);

        public override object Draw(Rect rect, GUIContent label, Type fieldType, object value)
        {
            if (value == null)
            {
                return null;
            }

            Type elementType = fieldType.GenericTypeArguments[0];

            if (value is not ISingleElementStack singleElementStack)
            {
                return value;
            }

            var labelRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            var buttonRect = new Rect(rect.x + EditorGUIUtility.labelWidth + 5, rect.y,
                rect.width - EditorGUIUtility.labelWidth - 5, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(labelRect, label);

            var currentElement = singleElementStack.GetObjectElement();
            var currentElementName = currentElement != null ? currentElement.GetType().Name : "No Element";

            if (GUI.Button(buttonRect, currentElementName))
            {
                ShowAddMenu(singleElementStack, elementType);
            }

            return singleElementStack;
        }

        private void ShowAddMenu(ISingleElementStack stack, Type elementType)
        {
            var menu = new GenericMenu();

            foreach (Type type in TypeHierarchyHelper.GetDerivedTypes(elementType))
            {
                var typeName = type.Name;

                menu.AddItem(new GUIContent(typeName), false, () => stack.ReplaceElement(type));
            }

            menu.ShowAsContext();
        }
    }
}
#endif
