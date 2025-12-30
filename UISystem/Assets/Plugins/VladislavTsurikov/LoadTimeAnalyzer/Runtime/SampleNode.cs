using System.Diagnostics;
using VladislavTsurikov.GraphRuntime.Runtime;

namespace VladislavTsurikov.LoadTimeAnalyzer.Runtime
{
    public class SampleNode : Node<SampleNode>
    {
        public string Name { get; }

        public Stopwatch Stopwatch { get; } = Stopwatch.StartNew();

        public bool IsStopped { get; private set; }

        public SampleNode(string name)
        {
            Name = name;
        }

        public void Stop()
        {
            if (!IsStopped)
            {
                Stopwatch.Stop();
                IsStopped = true;
            }
        }

        public long ElapsedMs => Stopwatch.ElapsedMilliseconds;
    }
}