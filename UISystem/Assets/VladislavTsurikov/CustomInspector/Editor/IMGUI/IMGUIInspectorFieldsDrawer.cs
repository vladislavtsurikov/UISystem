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

            ButtonDrawer.DrawButtons(target, ref rect);
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

                foreach (IMGUIDecoratorDrawer decorator in processedField.Decorators)
                {
                    float decoratorHeight = decorator.GetHeight(field, target);
                    Rect decoratorRect = new Rect(rect.x, rect.y, rect.width, decoratorHeight);

                    decorator.Draw(decoratorRect, field, target);

                    rect.y += decoratorHeight;
                    _totalHeight += decoratorHeight;
                }

                var fieldLabel = new GUIContent(processedField.FieldName, processedField.Tooltip);

                if (drawer != null)
                {
                    var fieldHeight = drawer.GetFieldsHeight(target, field, value);
                    Rect fieldRect = EditorGUI.IndentedRect(new Rect(rect.x, rect.y, rect.width, fieldHeight));

                    using (CreateFieldPresentationScope(
                               field,
                               target,
                               processedField.StateProcessors,
                               processedField.StyleProcessors,
                               null))
                    {
                        EditorGUI.BeginChangeCheck();

                        var newValue = drawer.Draw(fieldRect, fieldLabel, field, target, value);
                        bool uiChanged = EditorGUI.EndChangeCheck();
                        newValue = ApplyProcessorsAndAssignIfNeeded(
                            field,
                            target,
                            newValue,
                            value,
                            processedField.ValueProcessors,
                            uiChanged);
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
            _totalHeight += ButtonDrawer.GetButtonsHeight(target);
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
                foreach (IMGUIDecoratorDrawer decorator in processedField.Decorators)
                {
                    _totalHeight += decorator.GetHeight(processedField.Field, target);
                }

                if (processedField.Drawer != null)
                {
                    _totalHeight += processedField.Drawer.GetFieldsHeight(
                        target,
                        processedField.Field,
                        processedField.Value);
                }
                else
                {
                    _totalHeight += EditorGUIUtility.singleLineHeight;
                }
            }
        }

        protected override FieldPresentationScope CreateFieldPresentationScope(
            FieldState state,
            FieldStyle style,
            object fieldElement)
        {
            return new IMGUIFieldPresentationScope(state, style);
        }
    }
}
#endif
