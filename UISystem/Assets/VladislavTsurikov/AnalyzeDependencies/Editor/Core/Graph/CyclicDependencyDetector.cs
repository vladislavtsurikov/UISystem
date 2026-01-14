using System.Collections.Generic;
using System.Linq;
using System.Text;
using VladislavTsurikov.AnalyzeDependencies.Editor.Core.Models;

namespace VladislavTsurikov.AnalyzeDependencies.Editor.Core.Graph
{
    public class CyclicDependencyDetector
    {
        private readonly Dictionary<string, AssemblyInfo> _assemblies;
        private readonly Dictionary<string, string> _guidToName;
        private HashSet<string> _visited;
        private HashSet<string> _recursionStack;
        private List<CyclicDependency> _cycles;

        public CyclicDependencyDetector(DependencyAnalyzer analyzer)
        {
            _assemblies = analyzer.GetAssembliesDictionary();
            _guidToName = analyzer.GetGuidToNameMap();
        }

        public List<CyclicDependency> DetectCycles()
        {
            _cycles = new List<CyclicDependency>();
            _visited = new HashSet<string>();
            _recursionStack = new HashSet<string>();

            foreach (var assembly in _assemblies.Values)
            {
                if (!_visited.Contains(assembly.Guid))
                {
                    DetectCyclesDFS(assembly.Guid, new List<string>());
                }
            }

            return RemoveDuplicateCycles(_cycles);
        }

        private bool DetectCyclesDFS(string currentGuid, List<string> path)
        {
            if (_recursionStack.Contains(currentGuid))
            {
                int cycleStart = path.IndexOf(currentGuid);
                if (cycleStart >= 0)
                {
                    var cycle = new CyclicDependency();
                    for (int i = cycleStart; i < path.Count; i++)
                    {
                        cycle.Cycle.Add(path[i]);
                        cycle.CycleNames.Add(GetAssemblyName(path[i]));
                    }
                    cycle.Cycle.Add(currentGuid);
                    cycle.CycleNames.Add(GetAssemblyName(currentGuid));

                    _cycles.Add(cycle);
                }
                return true;
            }

            if (_visited.Contains(currentGuid))
                return false;

            _visited.Add(currentGuid);
            _recursionStack.Add(currentGuid);
            path.Add(currentGuid);

            var assembly = _assemblies.Values.FirstOrDefault(a => a.Guid == currentGuid);
            if (assembly != null)
            {
                foreach (var depGuid in assembly.Dependencies)
                {
                    if (_assemblies.Values.Any(a => a.Guid == depGuid))
                    {
                        DetectCyclesDFS(depGuid, new List<string>(path));
                    }
                }
            }

            _recursionStack.Remove(currentGuid);
            return false;
        }

        private List<CyclicDependency> RemoveDuplicateCycles(List<CyclicDependency> cycles)
        {
            var uniqueCycles = new List<CyclicDependency>();
            var seenCycles = new HashSet<string>();

            foreach (var cycle in cycles)
            {
                var sortedCycle = new List<string>(cycle.Cycle);
                sortedCycle.Sort();
                string cycleKey = string.Join("|", sortedCycle);

                if (!seenCycles.Contains(cycleKey))
                {
                    seenCycles.Add(cycleKey);
                    uniqueCycles.Add(cycle);
                }
            }

            return uniqueCycles;
        }

        private string GetAssemblyName(string guid) => _guidToName.ContainsKey(guid) ? _guidToName[guid] : guid;

        public string GenerateReport(List<CyclicDependency> cycles)
        {
            if (cycles.Count == 0)
            {
                return "✓ No cyclic dependencies found!";
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"⚠ Found {cycles.Count} cyclic dependencies:\n");

            for (int i = 0; i < cycles.Count; i++)
            {
                sb.AppendLine($"Cycle #{i + 1}:");
                sb.AppendLine($"  {cycles[i]}");
                sb.AppendLine();
            }

            sb.AppendLine("Cyclic dependencies can cause:");
            sb.AppendLine("- Increased compilation times");
            sb.AppendLine("- Difficulty in code maintenance");
            sb.AppendLine("- Potential runtime issues");
            sb.AppendLine("\nConsider refactoring to remove these cycles.");

            return sb.ToString();
        }
    }
}
