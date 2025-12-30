using System.Collections.Generic;

namespace VladislavTsurikov.GraphRuntime.Runtime
{
    public abstract class Graph<TNode> where TNode : Node<TNode>
    {
        public List<TNode> Roots { get; } = new();
        public List<TNode> Leaves { get; } = new();

        protected virtual void OnRootAdded(TNode node)
        {
        }

        protected virtual void OnLeafAdded(TNode node)
        {
        }

        protected virtual void OnCleared()
        {
        }

        public void AddRoot(TNode node)
        {
            Roots.Add(node);
            OnRootAdded(node);
        }

        public void AddLeaf(TNode node)
        {
            Leaves.Add(node);
            OnLeafAdded(node);
        }

        public void Clear()
        {
            Roots.Clear();
            Leaves.Clear();
            OnCleared();
        }
    }
}