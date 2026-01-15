#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Runtime;

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

                using var scope = InspectorContext.Push(target, field, elementIndex);

                foreach (UIToolkitDecoratorDrawer decorator in processedField.Decorators)
                {
                    var decoratorElement = decorator.CreateElement();
                    if (decoratorElement != null)
                    {
                        container.Add(decoratorElement);
                    }
                }

                if (drawer != null)
                {
                    var fieldElement = drawer.CreateField(fieldName, field.FieldType, value, newValue =>
                    {
                        field.SetValue(target, newValue);
                    });

                    bool isReadOnly = field.GetCustomAttribute<ReadOnlyAttribute>() != null;
                    bool isDisabled = EvaluateDisableIfCondition(field, target);
                    if (isReadOnly || isDisabled)
                    {
                        fieldElement.SetEnabled(false);
                    }

                    var guiColorAttribute = field.GetCustomAttribute<GUIColorAttribute>();
                    if (guiColorAttribute != null)
                    {
                        var color = guiColorAttribute.GetColor(target);
                        fieldElement.style.backgroundColor = new StyleColor(color);
                    }

                    if (!string.IsNullOrEmpty(processedField.Tooltip))
                    {
                        fieldElement.tooltip = processedField.Tooltip;
                    }

                    container.Add(fieldElement);
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

        private bool EvaluateDisableIfCondition(FieldInfo field, object target)
        {
            var disableIfAttribute = field.GetCustomAttribute<DisableIfAttribute>();
            if (disableIfAttribute == null)
            {
                return false;
            }

            FieldInfo conditionField = target.GetType().GetField(disableIfAttribute.ConditionMemberName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (conditionField == null)
            {
                return false;
            }

            object conditionValue = conditionField.GetValue(target);
            return IsTruthy(conditionValue);
        }

        private bool IsTruthy(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (value is bool boolValue)
            {
                return boolValue;
            }

            if (value is UnityEngine.Object unityObject)
            {
                return unityObject != null;
            }

            return true;
        }
    }
}
#endif
