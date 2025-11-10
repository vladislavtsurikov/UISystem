using System;
using System.Collections.Generic;

namespace VladislavTsurikov.IMGUIUtility.Runtime.ElementStack.IconStack
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DropObjectsAttribute : Attribute
    {
        public readonly List<Type> ObjectsTypes = new();

        public DropObjectsAttribute(Type type) => ObjectsTypes.Add(type);

        public DropObjectsAttribute(Type[] types) => ObjectsTypes.AddRange(types);
    }
}
