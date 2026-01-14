namespace VladislavTsurikov.AnalyzeDependencies.Editor.Core
{
    public abstract class DependencyTool
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract bool CanExecute(DependencyAnalyzer analyzer);
        public abstract void Execute(DependencyAnalyzer analyzer);

        public virtual void Setup(DependencyAnalyzer analyzer)
        {
            // Override in derived classes if initialization is needed
        }
    }
}
