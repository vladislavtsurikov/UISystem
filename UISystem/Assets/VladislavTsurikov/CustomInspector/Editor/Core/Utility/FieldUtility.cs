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

        public static object GetOrCreateTypeInstance(Type fieldType, FieldDrawer drawer)
        {
            if (fieldType == null)
            {
                return null;
            }

            if (drawer != null && drawer.ShouldCreateInstanceIfNull() == false)
            {
                return null;
            }

            return CreateDefaultValue(fieldType);
        }

        private static object CreateDefaultValue(Type type)
        {
            if (type == typeof(string))
            {
                return string.Empty;
            }

            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                return null;
            }

            if (type.GetConstructor(Type.EmptyTypes) != null)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }

        public static FieldInfo[] GetSerializableFields(Type targetType, BindingFlags bindingFlags,
            bool excludeInternal, Type[] excludedDeclaringTypes)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }

            var fields = new System.Collections.Generic.List<FieldInfo>();
            var typeHierarchy = new System.Collections.Generic.List<Type>();

            for (Type currentType = targetType; currentType != null; currentType = currentType.BaseType)
            {
                typeHierarchy.Add(currentType);
            }

            for (int i = typeHierarchy.Count - 1; i >= 0; i--)
            {
                Type currentType = typeHierarchy[i];
                FieldInfo[] typeFields = currentType.GetFields(bindingFlags | BindingFlags.DeclaredOnly);

                for (int fieldIndex = 0; fieldIndex < typeFields.Length; fieldIndex++)
                {
                    FieldInfo field = typeFields[fieldIndex];
                    if (!(field.IsPublic ||
                          field.IsDefined(typeof(SerializeField), false) ||
                          field.IsDefined(typeof(OdinSerializeAttribute), false)))
                    {
                        continue;
                    }

                    if (excludeInternal && field.IsAssembly)
                    {
                        continue;
                    }

                    if (excludedDeclaringTypes != null && excludedDeclaringTypes.Contains(field.DeclaringType))
                    {
                        continue;
                    }

                    fields.Add(field);
                }
            }

            return fields.ToArray();
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
