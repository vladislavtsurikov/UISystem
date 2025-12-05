using System;

namespace VladislavTsurikov.ActionFlow.Runtime.Events
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RequiredComponentsAttribute : Attribute
    {
        public RequiredComponentsAttribute(params Type[] requiredTypes) => RequiredTypes = requiredTypes;

        public Type[] RequiredTypes { get; }
    }
}
