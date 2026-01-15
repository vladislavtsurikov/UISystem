using System;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public abstract class DecoratorDrawerMatcher<TDrawer> where TDrawer : DecoratorDrawer
    {
        public abstract bool CanDraw(Attribute attribute);
        public abstract Type DrawerType { get; }
    }
}
