using System;
using System.Collections.Generic;
using System.Reflection;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public abstract class FieldDrawerMatcher<TDrawer> where TDrawer : FieldDrawer
    {
        public abstract bool CanDraw(FieldInfo field);
        public virtual IReadOnlyList<Type> AttributeTypes => Array.Empty<Type>();
        public abstract Type DrawerType { get; }
    }
}
