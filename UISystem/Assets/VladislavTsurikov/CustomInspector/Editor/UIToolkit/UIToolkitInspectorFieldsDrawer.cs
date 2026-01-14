#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    public class UIToolkitInspectorFieldsDrawer : InspectorFieldsDrawer<UIToolkitFieldDrawer>
    {
        private readonly UIToolkitRecursiveFieldsDrawer _recursiveFieldsDrawer = new();

        public UIToolkitInspectorFieldsDrawer(
            List<Type> excludedDeclaringTypes = null,
            bool excludeInternal = true,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            : base(excludedDeclaringTypes, excludeInternal, bindingFlags)
        {
        }

        public VisualElement CreateFieldsContainer(object target)
        {
            var container = new VisualElement();

            if (target == null)
            {
                container.Add(new Label("Target is null"));
                return container;
            }

            DrawFieldsRecursive(target, container);

            return container;
        }

        private void DrawFieldsRecursive(object target, VisualElement container)
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

                    container.Add(fieldElement);
                }
                else
                {
                    var recursiveElement = _recursiveFieldsDrawer.DrawRecursiveFields(
                        value,
                        field,
                        (obj, cont) => DrawFieldsRecursive(obj, cont));

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
