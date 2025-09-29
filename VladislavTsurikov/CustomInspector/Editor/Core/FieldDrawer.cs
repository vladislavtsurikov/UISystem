using System;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public abstract class FieldDrawer
    {
        public bool Foldout { get; set; } = false;

        public abstract bool CanDraw(Type fieldType);

        public virtual bool ShouldCreateInstanceIfNull() => true;
    }
}
