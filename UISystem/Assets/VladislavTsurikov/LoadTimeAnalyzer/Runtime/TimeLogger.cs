using System.Collections.Generic;

namespace VladislavTsurikov.LoadTimeAnalyzer.Runtime
{
    public static class TimeLogger
    {
        private static TimeLogTree _tree;
        private static readonly Stack<string> _contexts = new Stack<string>();

        public static Stack<string> Contexts => _contexts;

        public static void EnableSorting(bool enabled)
        {
            _tree?.EnableSorting(enabled);
        }

        public static void BeginContext(string rootSampleName)
        {
            if (_tree == null)
            {
                _tree = new TimeLogTree();
            }

            _contexts.Push(rootSampleName);
        }

        public static void EndContext()
        {
            if (_tree == null)
            {
                return;
            }

            if (_contexts.Count == 0)
            {
                return;
            }

            _contexts.Pop();

            if (_contexts.Count == 0)
            {
                _tree.LogResults();
                _tree.Clear();
                _tree = null;
            }
        }

        public static void BeginSample(string name, string parentName = null)
        {
            if (_tree == null)
            {
                _tree = new TimeLogTree();
            }

            _tree.BeginSample(name, parentName);
        }

        public static void EndSample(string name)
        {
            if (_tree != null)
            {
                _tree.EndSample(name);
            }
        }

        private static void LogResults()
        {
            if (_tree != null)
            {
                _tree.LogResults();
            }
        }

        public static void Clear()
        {
            if (_tree != null)
            {
                _tree.Clear();
                _tree = null;
            }

            _contexts.Clear();
        }
    }
}
