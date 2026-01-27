#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OdinSerializer.Utilities;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public abstract class InspectorFieldsDrawer<TFieldDrawer, TDecoratorDrawer>
        where TFieldDrawer : FieldDrawer
        where TDecoratorDrawer : DecoratorDrawer
    {
        private readonly BindingFlags _bindingFlags;
        private readonly List<Type> _excludedDeclaringTypes;
        private readonly bool _excludeInternal;
        private readonly Dictionary<Type, List<ProcessedField>> _cachedFields = new();

        protected InspectorFieldsDrawer(
            List<Type> excludedDeclaringTypes = null,
            bool excludeInternal = true,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            _excludedDeclaringTypes = excludedDeclaringTypes ?? new List<Type>();
            _excludeInternal = excludeInternal;
            _bindingFlags = bindingFlags;
        }

        protected IEnumerable<ProcessedField> GetProcessedFields(object target)
        {
            List<ProcessedField> processedFields = GetOrCreateProcessedFields(target.GetType());

            foreach (ProcessedField processedField in processedFields)
            {
                if (!IsFieldVisible(processedField.Field))
                {
                    continue;
                }

                if (!EvaluateShowIfCondition(processedField.Field, target))
                {
                    continue;
                }

                if (EvaluateHideIfCondition(processedField.Field, target))
                {
                    continue;
                }

                TFieldDrawer drawer = processedField.Drawer;

                var value = drawer == null || drawer.ShouldCreateInstanceIfNull()
                    ? FieldUtility.GetOrCreateFieldInstance(processedField.Field, target)
                    : processedField.Field.GetValue(target);

                processedField.Value = value;

                yield return processedField;
            }
        }

        private List<ProcessedField> GetOrCreateProcessedFields(Type targetType)
        {
            if (_cachedFields.TryGetValue(targetType, out List<ProcessedField> processedFields))
            {
                return processedFields;
            }

            FieldInfo[] fields = FieldUtility.GetSerializableFields(
                targetType,
                _bindingFlags,
                _excludeInternal,
                _excludedDeclaringTypes.ToArray()
            );

            fields = fields.OrderBy(field =>
            {
                var orderAttribute = field.GetCustomAttribute<OrderAttribute>();
                return orderAttribute?.Order ?? int.MaxValue;
            }).ToArray();

            processedFields = new List<ProcessedField>(fields.Length);

            foreach (FieldInfo field in fields)
            {
                TFieldDrawer drawer = FieldDrawerResolver<TFieldDrawer>.CreateDrawer(field.FieldType);
                string fieldName = FieldUtility.GetFieldLabel(field);

                List<TDecoratorDrawer> decorators = DecoratorDrawerResolver<TDecoratorDrawer>.CreateDrawers(field);

                var tooltipAttribute = field.GetCustomAttribute<TooltipAttribute>();
                string tooltip = tooltipAttribute?.tooltip ?? "";

                processedFields.Add(new ProcessedField(field, fieldName, drawer, decorators, tooltip));
            }

            _cachedFields[targetType] = processedFields;

            return processedFields;
        }

        private bool IsFieldVisible(FieldInfo field)
        {
            if (CustomInspectorPreferences.Instance.ShowFieldWithHideInInspectorAttribute)
            {
                return true;
            }

            return field.GetAttribute<HideInInspector>() == null;
        }

        private bool EvaluateShowIfCondition(FieldInfo field, object target)
        {
            var showIfAttribute = field.GetCustomAttribute<ShowIfAttribute>();
            if (showIfAttribute == null)
            {
                return true;
            }

            FieldInfo conditionField = target.GetType().GetField(showIfAttribute.ConditionMemberName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (conditionField == null)
            {
                return true;
            }

            object conditionValue = conditionField.GetValue(target);
            bool result = IsTruthy(conditionValue);

            return showIfAttribute.Value == result;
        }

        private bool EvaluateHideIfCondition(FieldInfo field, object target)
        {
            var hideIfAttribute = field.GetCustomAttribute<HideIfAttribute>();
            if (hideIfAttribute == null)
            {
                return false;
            }

            FieldInfo conditionField = target.GetType().GetField(hideIfAttribute.ConditionMemberName,
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

        protected sealed class ProcessedField
        {
            public ProcessedField(FieldInfo field, string fieldName, TFieldDrawer drawer, List<TDecoratorDrawer> decorators, string tooltip)
            {
                Field = field;
                FieldName = fieldName;
                Drawer = drawer;
                Decorators = decorators;
                Tooltip = tooltip;
            }

            public FieldInfo Field { get; }
            public string FieldName { get; }
            public TFieldDrawer Drawer { get; }
            public List<TDecoratorDrawer> Decorators { get; }
            public string Tooltip { get; }
            public object Value { get; set; }
        }
    }
}
#endif
