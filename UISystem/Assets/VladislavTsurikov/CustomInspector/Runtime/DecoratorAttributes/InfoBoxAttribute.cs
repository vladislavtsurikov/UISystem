using System;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class InfoBoxAttribute : Attribute
    {
        public InfoBoxAttribute(string message, InfoBoxMessageType messageType = InfoBoxMessageType.Info)
        {
            Message = message;
            MessageType = messageType;
            MessageMemberName = null;
            VisibleIfMemberName = null;
        }

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

        public string VisibleIfMemberName { get; set; }

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

            var type = target.GetType();

            var field = type.GetField(MessageMemberName,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (field != null)
            {
                return field.GetValue(target)?.ToString() ?? string.Empty;
            }

            var property = type.GetProperty(MessageMemberName,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (property != null)
            {
                return property.GetValue(target)?.ToString() ?? string.Empty;
            }

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

        public bool IsVisible(object target)
        {
            if (string.IsNullOrWhiteSpace(VisibleIfMemberName))
            {
                return true;
            }

            var type = target.GetType();

            var field = type.GetField(VisibleIfMemberName,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (field != null)
            {
                return IsTruthy(field.GetValue(target));
            }

            var property = type.GetProperty(VisibleIfMemberName,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (property != null)
            {
                return IsTruthy(property.GetValue(target));
            }

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
