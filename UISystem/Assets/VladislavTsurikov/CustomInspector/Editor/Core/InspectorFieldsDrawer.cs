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
            FieldInfo[] fields = FieldUtility.GetSerializableFields(
                target.GetType(),
                _bindingFlags,
                _excludeInternal,
                _excludedDeclaringTypes.ToArray()
            );

            foreach (FieldInfo field in fields)
            {
                if (!IsFieldVisible(field))
                {
                    continue;
                }

                TDrawer drawer = FieldDrawerResolver<TDrawer>.GetFieldDrawer(field.FieldType);

                var value = drawer == null || drawer.ShouldCreateInstanceIfNull()
                    ? FieldUtility.GetOrCreateFieldInstance(field, target)
                    : field.GetValue(target);

                var fieldName = FieldUtility.GetFieldLabel(field);

                yield return (drawer, field, fieldName, value);
            }
        }

        private bool IsFieldVisible(FieldInfo field)
        {
            if (CustomInspectorPreferences.Instance.ShowFieldWithHideInInspectorAttribute)
            {
                return true;
            }

            return field.GetAttribute<HideInInspector>() == null;
        }
    }
}
#endif
