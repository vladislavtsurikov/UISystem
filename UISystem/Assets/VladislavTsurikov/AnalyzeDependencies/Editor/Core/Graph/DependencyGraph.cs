using System;
using System.Collections.Generic;
using VladislavTsurikov.AnalyzeDependencies.Editor.Core.Models;

namespace VladislavTsurikov.AnalyzeDependencies.Editor.Core.Graph
{
    [Serializable]
    public class DependencyGraph
    {
        public List<AssemblyInfo> Nodes = new List<AssemblyInfo>();
        public List<DependencyEdge> Edges = new List<DependencyEdge>();
        public int TotalAssemblies;
        public int TotalDependencies;
        public int UnusedDependencies;
    }
}
