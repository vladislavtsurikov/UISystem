#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

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

            foreach ((UIToolkitFieldDrawer drawer, FieldInfo field, var fieldName, var value) in GetProcessedFields(target))
            {
                if (drawer != null)
                {
                    var fieldElement = drawer.CreateField(fieldName, field.FieldType, value, newValue =>
                    {
                        field.SetValue(target, newValue);
                    });

                    container.Add(fieldElement);
                }
                else
                {
                    var recursiveElement = _recursiveFieldsDrawer.DrawRecursiveFields(
                        value,
                        field,
                        DrawFieldsRecursive);

                    container.Add(recursiveElement);
                }
            }
        }
    }
}
#endif
