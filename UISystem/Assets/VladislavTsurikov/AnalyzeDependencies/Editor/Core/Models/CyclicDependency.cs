using System.Collections.Generic;

namespace VladislavTsurikov.AnalyzeDependencies.Editor.Core.Models
{
    public class CyclicDependency
    {
        public List<string> Cycle { get; set; } = new List<string>();
        public List<string> CycleNames { get; set; } = new List<string>();

        public override string ToString() => string.Join(" → ", CycleNames) + " → " + CycleNames[0];
    }
}
