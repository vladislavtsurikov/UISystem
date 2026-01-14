using System;
using System.Collections.Generic;

namespace VladislavTsurikov.Nody.Runtime.AdvancedNodeStack
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CreateNodesAttribute : Attribute
    {
        public readonly List<Type> Types;

        public CreateNodesAttribute(Type type) => Types = new List<Type> { type };

        public CreateNodesAttribute(Type[] types) => Types = new List<Type>(types);
    }
}
