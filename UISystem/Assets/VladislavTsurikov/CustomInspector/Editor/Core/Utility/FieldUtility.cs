using System;
using System.Linq;
using System.Reflection;
using OdinSerializer;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public static class FieldUtility
    {
        public static object GetOrCreateFieldInstance(FieldInfo field, object target)
        {
            var value = field.GetValue(target);

            if (value == null && field.FieldType.IsClass)
            {
                ConstructorInfo constructor = field.FieldType.GetConstructor(Type.EmptyTypes);

                if (constructor != null)
                {
                    value = Activator.CreateInstance(field.FieldType);
                    field.SetValue(target, value);
                }
            }

            return value;
        }

        public static FieldInfo[] GetSerializableFields(Type targetType, BindingFlags bindingFlags,
            bool excludeInternal, Type[] excludedDeclaringTypes)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }

            return targetType
                .GetFields(bindingFlags)
                .Where(field =>
                    (field.IsPublic ||
                     field.IsDefined(typeof(SerializeField), false) ||
                     field.IsDefined(typeof(OdinSerializeAttribute), false)) &&
                    (!excludeInternal || !field.IsAssembly) &&
                    (excludedDeclaringTypes == null || !excludedDeclaringTypes.Contains(field.DeclaringType)))
                .ToArray();
        }

#if UNITY_EDITOR
        public static string GetFieldLabel<T>(string fieldName)
        {
            FieldInfo field = typeof(T).GetField(fieldName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field == null)
            {
                throw new ArgumentException($"Field '{fieldName}' not found in type '{typeof(T).Name}'.");
            }

            return GetFieldLabel(field);
        }

        public static string GetFieldLabel(FieldInfo field)
        {
            NameAttribute nameAttribute = field.GetCustomAttribute<NameAttribute>();
            return nameAttribute?.Name ?? ObjectNames.NicifyVariableName(field.Name);
        }
#endif
    }
}
