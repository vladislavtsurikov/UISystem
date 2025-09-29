using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace VladislavTsurikov.ReflectionUtility.Runtime
{
    public static class TypeExtensions
    {
        public static Type TryGetGenericArgument(this Type type, Type genericType, int argumentIndex = 0)
        {
            while (type != null && type != typeof(object))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == genericType)
                {
                    Type[] arguments = type.GetGenericArguments();
                    if (argumentIndex >= 0 && argumentIndex < arguments.Length)
                    {
                        return arguments[argumentIndex];
                    }

                    Debug.LogError(
                        $"TryGetGenericArgument: Invalid argument index {argumentIndex} for type {type.FullName}.");
                    return null;
                }

                type = type.BaseType;
            }

            return null;
        }

        public static bool HasParameterlessConstructor(this Type type) =>
            type
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Any(c => c.GetParameters().Length == 0);
    }
}
