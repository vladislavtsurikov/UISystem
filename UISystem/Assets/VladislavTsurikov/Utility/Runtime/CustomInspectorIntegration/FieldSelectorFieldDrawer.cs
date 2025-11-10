#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;
using VladislavTsurikov.Utility.Runtime.CustomInspectorIntegration;

namespace QuestsSystem.IntegrationActionFlow.Pointer
{
    public class FieldSelectorFieldDrawer : IMGUIFieldDrawer
    {
        public override bool CanDraw(Type type) => typeof(FieldSelector).IsAssignableFrom(type);

        public override object Draw(Rect rect, GUIContent label, Type fieldType, object value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is not FieldSelector fieldSelector)
            {
                return value;
            }

            var labelRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            var buttonRect = new Rect(rect.x + EditorGUIUtility.labelWidth + 5, rect.y,
                rect.width - EditorGUIUtility.labelWidth - 5, EditorGUIUtility.singleLineHeight);

            if (fieldSelector.DeclaringType == null)
            {
                EditorGUI.LabelField(labelRect, "DeclaringType is not set");
                return value;
            }

            EditorGUI.LabelField(labelRect, label);

            var currentFieldName = fieldSelector.FieldInfo?.Name ?? "(None)";
            if (GUI.Button(buttonRect, currentFieldName))
            {
                OpenFieldSelectorPopup(buttonRect, fieldSelector);
            }

            return value;
        }

        private void OpenFieldSelectorPopup(Rect buttonRect, FieldSelector fieldSelector)
        {
            List<FieldInfo> fields = GetValidFields(fieldSelector.DeclaringType, fieldSelector);

            TypeSelectorPopup.Open(
                buttonRect,
                "Select Field",
                fields,
                field => field.Name,
                selectedField => fieldSelector.FieldName = selectedField.Name
            );
        }

        private List<FieldInfo> GetValidFields(Type declaringType, FieldSelector fieldSelector)
        {
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            return declaringType.GetFields(bindingFlags)
                .Where(field => (field.IsPrivate || field.IsPublic) && fieldSelector.IsValidFieldType(field.FieldType))
                .ToList();
        }
    }
}
#endif
