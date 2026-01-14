using System;
using System.Collections.Generic;

namespace VladislavTsurikov.AnalyzeDependencies.Editor.Core.Models
{
    [Serializable]
    public class AssemblyInfo
    {
        public string Name { get; set; }
        public string Guid { get; set; }
        public string Path { get; set; }
        public List<string> Dependencies { get; set; }
        public List<string> UnusedDependencies { get; set; }
        public List<string> UsedBy { get; set; }
        public float Centrality { get; set; }
        public bool IsSelected { get; set; }

        public int UsageCount => UsedBy?.Count ?? 0;
        public int DependencyCount => Dependencies?.Count ?? 0;

        public AssemblyInfo()
        {
            Dependencies = new List<string>();
            UnusedDependencies = new List<string>();
            UsedBy = new List<string>();
            IsSelected = false;
        }
    }
}
