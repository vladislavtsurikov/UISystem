using System;
using UnityEngine.UIElements;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class HelpBoxAttribute : Attribute
    {
        public HelpBoxAttribute(string message, HelpBoxMessageType messageType = HelpBoxMessageType.Info)
        {
            Message = message;
            MessageType = messageType;
        }

        public string Message { get; }
        public HelpBoxMessageType MessageType { get; }
    }
}
