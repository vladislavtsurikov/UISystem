using System;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public abstract class FieldDrawerMatcher<TDrawer> where TDrawer : FieldDrawer
    {
        public abstract bool CanDraw(Type fieldType);
        public abstract Type DrawerType { get; }
    }
}
