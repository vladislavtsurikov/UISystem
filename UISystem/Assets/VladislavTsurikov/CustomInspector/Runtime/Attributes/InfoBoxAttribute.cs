using System;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    /// <summary>
    /// Displays an information box in the inspector.
    /// Enhanced version of HelpBoxAttribute with support for dynamic messages and conditional visibility.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class InfoBoxAttribute : Attribute
    {
        /// <summary>
        /// Create an info box with a static message
        /// </summary>
        public InfoBoxAttribute(string message, InfoBoxMessageType messageType = InfoBoxMessageType.Info)
        {
            Message = message;
            MessageType = messageType;
            MessageMemberName = null;
            VisibleIfMemberName = null;
        }

        /// <summary>
        /// Create an info box with a dynamic message from a member (field/property/method)
        /// </summary>
        public InfoBoxAttribute(string messageMemberName, InfoBoxMessageType messageType, bool isDynamic)
        {
            Message = null;
            MessageType = messageType;
            MessageMemberName = isDynamic ? messageMemberName : null;
            Message = isDynamic ? null : messageMemberName;
            VisibleIfMemberName = null;
        }

        public string Message { get; }
        public InfoBoxMessageType MessageType { get; }
        public string MessageMemberName { get; }

        /// <summary>
        /// Optional condition member name for conditional visibility
        /// </summary>
        public string VisibleIfMemberName { get; set; }

        /// <summary>
        /// Get the message to display (static or from member)
        /// </summary>
        public string GetMessage(object target)
        {
            if (!string.IsNullOrWhiteSpace(Message))
            {
                return Message;
            }

            if (string.IsNullOrWhiteSpace(MessageMemberName))
            {
                return string.Empty;
            }

            // Try to get message from field/property/method
            var type = target.GetType();

            // Try field
            var field = type.GetField(MessageMemberName,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (field != null)
            {
                return field.GetValue(target)?.ToString() ?? string.Empty;
            }

            // Try property
            var property = type.GetProperty(MessageMemberName,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (property != null)
            {
                return property.GetValue(target)?.ToString() ?? string.Empty;
            }

            // Try method
            var method = type.GetMethod(MessageMemberName,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance,
                null,
                Type.EmptyTypes,
                null);

            if (method != null)
            {
                return method.Invoke(target, null)?.ToString() ?? string.Empty;
            }

            return string.Empty;
        }

        /// <summary>
        /// Check if the info box should be visible based on VisibleIfMemberName condition
        /// </summary>
        public bool IsVisible(object target)
        {
            if (string.IsNullOrWhiteSpace(VisibleIfMemberName))
            {
                return true;
            }

            var type = target.GetType();

            // Try field
            var field = type.GetField(VisibleIfMemberName,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (field != null)
            {
                return IsTruthy(field.GetValue(target));
            }

            // Try property
            var property = type.GetProperty(VisibleIfMemberName,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (property != null)
            {
                return IsTruthy(property.GetValue(target));
            }

            // Try method
            var method = type.GetMethod(VisibleIfMemberName,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance,
                null,
                Type.EmptyTypes,
                null);

            if (method != null)
            {
                return IsTruthy(method.Invoke(target, null));
            }

            return true;
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

    public enum InfoBoxMessageType
    {
        None,
        Info,
        Warning,
        Error
    }
}
