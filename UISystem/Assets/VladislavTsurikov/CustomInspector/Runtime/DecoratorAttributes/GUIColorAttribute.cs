using System;
using UnityEngine;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class GUIColorAttribute : Attribute
    {
        public GUIColorAttribute(float r, float g, float b, float a = 1f)
        {
            Color = new Color(r, g, b, a);
            ColorMemberName = null;
        }

        public GUIColorAttribute(string colorMemberName)
        {
            Color = Color.white;
            ColorMemberName = colorMemberName;
        }

        public Color Color { get; }
        public string ColorMemberName { get; }

        public Color GetColor(object target)
        {
            if (string.IsNullOrWhiteSpace(ColorMemberName))
            {
                return Color;
            }

            var type = target.GetType();

            var field = type.GetField(ColorMemberName,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (field != null && field.FieldType == typeof(Color))
            {
                return (Color)field.GetValue(target);
            }

            var property = type.GetProperty(ColorMemberName,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (property != null && property.PropertyType == typeof(Color))
            {
                return (Color)property.GetValue(target);
            }

            var method = type.GetMethod(ColorMemberName,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance,
                null,
                Type.EmptyTypes,
                null);

            if (method != null && method.ReturnType == typeof(Color))
            {
                return (Color)method.Invoke(target, null);
            }

            return Color;
        }
    }
}
