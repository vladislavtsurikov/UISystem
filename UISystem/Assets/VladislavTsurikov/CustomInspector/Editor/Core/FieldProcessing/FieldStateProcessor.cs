using System;
using System.Reflection;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public abstract class FieldStateProcessor
    {
        public Attribute Attribute { get; private set; }

        public virtual void Initialize(Attribute attribute)
        {
            Attribute = attribute;
        }

        public abstract void Apply(FieldInfo field, object target, FieldState state);
    }
}

