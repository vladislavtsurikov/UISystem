using System;

namespace VladislavTsurikov.AnalyzeDependencies.Editor.Core.Models
{
    [Serializable]
    public class DependencyEdge
    {
        public string FromGuid;
        public string ToGuid;
        public string FromName;
        public string ToName;
        public bool IsUnused;
    }
}
