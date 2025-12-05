using System;
using System.Collections.Generic;

namespace VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CreateComponentsAttribute : Attribute
    {
        public readonly List<Type> Types;

        public CreateComponentsAttribute(Type type) => Types = new List<Type> { type };

        public CreateComponentsAttribute(Type[] types) => Types = new List<Type>(types);
    }
}
