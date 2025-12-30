using System;

namespace VladislavTsurikov.EntityDataActionFramework
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RequiresDataAttribute : Attribute
    {
        public readonly Type[] RequiredTypes;

        public RequiresDataAttribute(params Type[] requiredTypes)
        {
            RequiredTypes = requiredTypes;
        }
    }
}
