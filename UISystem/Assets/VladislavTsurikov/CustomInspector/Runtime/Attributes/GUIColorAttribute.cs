using System;
using UnityEngine;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    /// <summary>
    /// Applies a color tint to the field in the inspector.
    /// You can use RGB (0-1), string color name, or reference a field/property/method returning Color.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class GUIColorAttribute : Attribute
    {
        /// <summary>
        /// Create a GUI color from RGB values (0-1 range)
        /// </summary>
        public GUIColorAttribute(float r, float g, float b, float a = 1f)
        {
            Color = new Color(r, g, b, a);
            ColorMemberName = null;
        }

        /// <summary>
        /// Create a GUI color from a member name (field/property/method returning Color)
        /// </summary>
        public GUIColorAttribute(string colorMemberName)
        {
            Color = Color.white;
            ColorMemberName = colorMemberName;
        }

        public Color Color { get; }
        public string ColorMemberName { get; }

        /// <summary>
        /// Try to get color from member if ColorMemberName is specified
        /// </summary>
        public Color GetColor(object target)
        {
            if (string.IsNullOrWhiteSpace(ColorMemberName))
            {
                return Color;
            }

            // Try to get color from field/property/method
            var type = target.GetType();

            // Try field
            var field = type.GetField(ColorMemberName,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (field != null && field.FieldType == typeof(Color))
            {
                return (Color)field.GetValue(target);
            }

            // Try property
            var property = type.GetProperty(ColorMemberName,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (property != null && property.PropertyType == typeof(Color))
            {
                return (Color)property.GetValue(target);
            }

            // Try method
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

            // Fallback to default color
            return Color;
        }
    }
}
