using System;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public abstract class DecoratorDrawer
    {
        public Attribute Attribute { get; private set; }

        public void Initialize(Attribute attribute)
        {
            Attribute = attribute;
        }
    }
}
