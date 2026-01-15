using System;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class RequiredAttribute : Attribute
    {
        public RequiredAttribute(string message = null)
        {
            Message = message ?? "This field is required!";
        }

        public string Message { get; }
    }
}
