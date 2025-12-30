#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using OdinSerializer.Utilities;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public abstract class InspectorFieldsDrawer<TDrawer>
        where TDrawer : FieldDrawer
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

        protected IEnumerable<(TDrawer fieldDrawer, FieldInfo field, string fieldName, object value)>
            GetProcessedFields(object target)
        {
            List<ProcessedField> processedFields = GetOrCreateProcessedFields(target.GetType());

            foreach (ProcessedField processedField in processedFields)
            {
                if (!IsFieldVisible(processedField.Field))
                {
                    continue;
                }

                TDrawer drawer = processedField.Drawer;

                var value = drawer == null || drawer.ShouldCreateInstanceIfNull()
                    ? FieldUtility.GetOrCreateFieldInstance(processedField.Field, target)
                    : processedField.Field.GetValue(target);

                yield return (drawer, processedField.Field, processedField.FieldName, value);
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

            processedFields = new List<ProcessedField>(fields.Length);

            foreach (FieldInfo field in fields)
            {
                TDrawer drawer = FieldDrawerResolver<TDrawer>.CreateDrawer(field.FieldType);
                string fieldName = FieldUtility.GetFieldLabel(field);
                processedFields.Add(new ProcessedField(field, fieldName, drawer));
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

        private sealed class ProcessedField
        {
            public ProcessedField(FieldInfo field, string fieldName, TDrawer drawer)
            {
                Field = field;
                FieldName = fieldName;
                Drawer = drawer;
            }

            public FieldInfo Field { get; }
            public string FieldName { get; }
            public TDrawer Drawer { get; }
        }
    }
}
#endif
