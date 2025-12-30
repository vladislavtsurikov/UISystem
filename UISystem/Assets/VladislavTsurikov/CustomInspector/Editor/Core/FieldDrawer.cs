namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public abstract class FieldDrawer
    {
        public bool Foldout { get; set; } = false;

        public virtual bool ShouldCreateInstanceIfNull() => true;
    }
}
