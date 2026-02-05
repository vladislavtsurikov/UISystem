using System;
using System.Reflection;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public abstract class FieldValueProcessor
    {
        public Attribute Attribute { get; private set; }

        public virtual void Initialize(Attribute attribute)
        {
            Attribute = attribute;
        }

        public abstract object Process(FieldInfo field, object target, object value);
    }
}
