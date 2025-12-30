using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VladislavTsurikov.GraphRuntime.Runtime;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.LoadTimeAnalyzer.Runtime
{
    public class TimeLogTree : Graph<SampleNode>
    {
        private SampleNode _current;
        private bool _sortChildrenByElapsed;
        private readonly Dictionary<string, SampleNode> _allNodes = new();
        private readonly Dictionary<string, SampleNode> _activeSamples = new();

        protected override void OnCleared()
        {
            _current = null;
            _allNodes.Clear();
        }

        public void EnableSorting(bool enabled)
        {
            _sortChildrenByElapsed = enabled;
        }

        public void BeginSample(string name, string parentName = null)
        {
            SampleNode node = new SampleNode(name);

            if (!string.IsNullOrEmpty(parentName) && _allNodes.TryGetValue(parentName, out SampleNode parent))
            {
                node.SetParent(parent);
            }
            else if (_current != null)
            {
                node.SetParent(_current);
            }
            else
            {
                AddRoot(node);
            }

            _allNodes[name] = node;
            _activeSamples[name] = node;

            _current = node;
        }

        public void EndSample(string name)
        {
            if (_activeSamples.TryGetValue(name, out SampleNode node))
            {
                node.Stop();
                _activeSamples.Remove(name);

                if (node.Children.Count == 0)
                {
                    AddLeaf(node);
                }

                if (_current == node)
                {
                    _current = node.Parent;
                }
            }
            else
            {
                Debug.LogWarning($"[TimeLogger] EndSample called for '{name}' but sample not found.");
            }
        }

        public void LogResults()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("[TimeLogger] Load timing breakdown:");

            foreach (SampleNode root in Roots)
            {
                long totalTime = root.ElapsedMs;
                AppendRecursive(builder, root, totalTime, 0);
            }

            Debug.Log(builder.ToString());
        }

        private void AppendRecursive(StringBuilder builder, SampleNode node, long rootMs, int indent)
        {
            long ownTime = node.ElapsedMs;
            float percent = rootMs > 0 ? (ownTime / (float)rootMs) * 100f : 0f;
            percent = Math.Min(percent, 999.99f);

            string indentStr = new string(' ', indent * 2);
            string readableTime = TimeSpan.FromMilliseconds(ownTime).ToReadableDetailed();

            builder.AppendLine($"{indentStr}{node.Name}: {ownTime} ms ({readableTime}) ({percent:F2}%)");

            List<SampleNode> children = _sortChildrenByElapsed
                ? node.Children.OrderByDescending(c => c.ElapsedMs).ToList()
                : node.Children;

            foreach (SampleNode child in children)
            {
                AppendRecursive(builder, child, rootMs, indent + 1);
            }
        }
    }
}
