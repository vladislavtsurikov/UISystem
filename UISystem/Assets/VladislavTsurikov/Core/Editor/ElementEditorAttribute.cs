using System;

namespace VladislavTsurikov.Core.Editor
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ElementEditorAttribute : Attribute
    {
        public readonly Type Type;

        public ElementEditorAttribute(Type type) => Type = type;
    }
}
