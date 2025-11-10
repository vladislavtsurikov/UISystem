using System;

namespace VladislavTsurikov.ActionFlow.Runtime.Events
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventCallbacksTypeAttribute : Attribute
    {
        public readonly Type Type;

        public EventCallbacksTypeAttribute(Type type) => Type = type;
    }
}
