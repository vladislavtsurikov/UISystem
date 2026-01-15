#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public class IMGUIInspectorFieldsDrawer : InspectorFieldsDrawer<IMGUIFieldDrawer, IMGUIDecoratorDrawer>
    {
        private readonly IMGUIRecursiveFieldsDrawer _imguiRecursiveFieldsDrawer = new();

        private float _totalHeight;

        public IMGUIInspectorFieldsDrawer(
            List<Type> excludedDeclaringTypes = null,
            bool excludeInternal = true,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            : base(excludedDeclaringTypes, excludeInternal, bindingFlags)
        {
        }

        public void DrawFields(object target, Rect rect, int? elementIndex = null)
        {
            if (target == null)
            {
                EditorGUI.LabelField(rect, "Target is null");
                return;
            }

            _totalHeight = 0;

            DrawFieldsRecursive(target, rect, elementIndex);
        }

        private void DrawFieldsRecursive(object target, Rect rect, int? elementIndex)
        {
            if (target == null)
            {
                return;
            }

            foreach (var processedField in GetProcessedFields(target))
            {
                IMGUIFieldDrawer drawer = processedField.Drawer;
                FieldInfo field = processedField.Field;
                object value = processedField.Value;

                using var scope = InspectorContext.Push(target, field, elementIndex);

                foreach (IMGUIDecoratorDrawer decorator in processedField.Decorators)
                {
                    float decoratorHeight = decorator.GetHeight();
                    Rect decoratorRect = new Rect(rect.x, rect.y, rect.width, decoratorHeight);

                    decorator.Draw(decoratorRect);

                    rect.y += decoratorHeight;
                    _totalHeight += decoratorHeight;
                }

                var fieldLabel = new GUIContent(processedField.FieldName, processedField.Tooltip);

                if (drawer != null)
                {
                    var fieldHeight = drawer.GetFieldsHeight(value);
                    Rect fieldRect = EditorGUI.IndentedRect(new Rect(rect.x, rect.y, rect.width, fieldHeight));

                    EditorGUI.BeginChangeCheck();

                    var newValue = drawer.Draw(fieldRect, fieldLabel, field, value);

                    if (EditorGUI.EndChangeCheck())
                    {
                        field.SetValue(target, newValue);
                    }

                    rect.y += fieldHeight;
                    _totalHeight += fieldHeight;
                }
                else
                {
                    var recursiveFieldsHeight =
                        _imguiRecursiveFieldsDrawer.DrawRecursiveFields(value, field, rect,
                            (nestedTarget, nestedRect) => DrawFieldsRecursive(nestedTarget, nestedRect, elementIndex));

                    rect.y += recursiveFieldsHeight;
                    _totalHeight += recursiveFieldsHeight;
                }
            }
        }

        public float GetFieldsHeight(object target, int? elementIndex = null)
        {
            if (target == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            _totalHeight = 0;
            CalculateFieldsHeight(target, elementIndex);
            return _totalHeight;
        }

        private void CalculateFieldsHeight(object target, int? elementIndex)
        {
            if (target == null)
            {
                return;
            }

            foreach (var processedField in GetProcessedFields(target))
            {
                using var scope = InspectorContext.Push(target, processedField.Field, elementIndex);

                foreach (IMGUIDecoratorDrawer decorator in processedField.Decorators)
                {
                    _totalHeight += decorator.GetHeight();
                }

                if (processedField.Drawer != null)
                {
                    _totalHeight += processedField.Drawer.GetFieldsHeight(processedField.Value);
                }
                else
                {
                    _totalHeight += EditorGUIUtility.singleLineHeight;
                }
            }
        }
    }
}
#endif
