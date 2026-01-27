using System;

namespace VladislavTsurikov.EntityDataAction.Runtime.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class RunOnDirtyDataAttribute : Attribute
    {
        public Type[] DataTypes { get; }

        public RunOnDirtyDataAttribute(params Type[] dataTypes)
        {
            DataTypes = dataTypes ?? Array.Empty<Type>();
        }
    }
}
