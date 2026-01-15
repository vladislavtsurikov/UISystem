using System;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public abstract class DecoratorDrawer
    {
        public Attribute Attribute { get; private set; }

        public virtual void Initialize(Attribute attribute)
        {
            Attribute = attribute;
        }
    }
}
