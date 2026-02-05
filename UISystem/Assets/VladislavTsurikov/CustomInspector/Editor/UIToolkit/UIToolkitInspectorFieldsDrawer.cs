#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    public class UIToolkitInspectorFieldsDrawer : InspectorFieldsDrawer<UIToolkitFieldDrawer, UIToolkitDecoratorDrawer>
    {
        private readonly UIToolkitRecursiveFieldsDrawer _recursiveFieldsDrawer = new();

        public UIToolkitInspectorFieldsDrawer(
            List<Type> excludedDeclaringTypes = null,
            bool excludeInternal = true,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            : base(excludedDeclaringTypes, excludeInternal, bindingFlags)
        {
        }

        public VisualElement CreateFieldsContainer(object target, int? elementIndex = null)
        {
            var container = new VisualElement();

            if (target == null)
            {
                container.Add(new Label("Target is null"));
                return container;
            }

            DrawFieldsRecursive(target, container, elementIndex);

            return container;
        }

        private void DrawFieldsRecursive(object target, VisualElement container, int? elementIndex)
        {
            if (target == null)
            {
                return;
            }

            foreach (var processedField in GetProcessedFields(target))
            {
                UIToolkitFieldDrawer drawer = processedField.Drawer;
                FieldInfo field = processedField.Field;
                string fieldName = processedField.FieldName;
                object value = processedField.Value;

                foreach (UIToolkitDecoratorDrawer decorator in processedField.Decorators)
                {
                    var decoratorElement = decorator.CreateElement(field, target);
                    if (decoratorElement != null)
                    {
                        container.Add(decoratorElement);
                    }
                }

                if (drawer != null)
                {
                    var processedValue = ApplyProcessorsAndAssignIfNeeded(
                        field,
                        target,
                        value,
                        value,
                        processedField.ValueProcessors,
                        false);
                    value = processedValue;

                    var fieldElement = drawer.CreateField(fieldName, field.FieldType, value, newValue =>
                    {
                        ApplyProcessorsAndAssignIfNeeded(
                            field,
                            target,
                            newValue,
                            newValue,
                            processedField.ValueProcessors,
                            true);
                    });

                    using (CreateFieldPresentationScope(
                               field,
                               target,
                               processedField.StateProcessors,
                               processedField.StyleProcessors,
                               fieldElement))
                    {
                        if (!string.IsNullOrEmpty(processedField.Tooltip))
                        {
                            fieldElement.tooltip = processedField.Tooltip;
                        }

                        container.Add(fieldElement);
                    }
                }
                else
                {
                    var recursiveElement = _recursiveFieldsDrawer.DrawRecursiveFields(
                        value,
                        field,
                        (nestedTarget, nestedContainer) => DrawFieldsRecursive(nestedTarget, nestedContainer, elementIndex));

                    container.Add(recursiveElement);
                }
            }
        }

        protected override FieldPresentationScope CreateFieldPresentationScope(
            FieldState state,
            FieldStyle style,
            object fieldElement)
        {
            return new UIToolkitFieldPresentationScope(state, style, fieldElement as VisualElement);
        }
    }
}
#endif
