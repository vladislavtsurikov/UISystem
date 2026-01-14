using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.AnalyzeDependencies.Editor.Core
{
    public class DependencyToolManager
    {
        private readonly List<DependencyTool> _tools;

        public DependencyToolManager()
        {
            _tools = ReflectionFactory.CreateAllInstances<DependencyTool>().ToList();
        }

        public IEnumerable<DependencyTool> GetTools() => _tools;

        public T GetTool<T>() where T : DependencyTool
        {
            return _tools.OfType<T>().FirstOrDefault();
        }

        public DependencyTool GetToolByName(string name)
        {
            return _tools.FirstOrDefault(t => t.Name == name);
        }
    }
}
