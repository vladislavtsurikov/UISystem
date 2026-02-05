using System;
using System.Reflection;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public abstract class FieldVisibilityProcessor
    {
        public Attribute Attribute { get; private set; }

        public virtual void Initialize(Attribute attribute)
        {
            Attribute = attribute;
        }

        public abstract bool IsVisible(FieldInfo field, object target);
    }
}

