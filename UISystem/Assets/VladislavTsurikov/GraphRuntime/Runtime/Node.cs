using System.Collections.Generic;

namespace VladislavTsurikov.GraphRuntime.Runtime
{
    public abstract class Node<TNode> where TNode : Node<TNode>
    {
        public TNode Parent { get; private set; }
        public List<TNode> Children { get; } = new();

        public void SetParent(TNode parent)
        {
            Parent = parent;
            parent?.Children.Add((TNode)this);
        }

        public IReadOnlyList<TNode> GetChildren() => Children;
    }
}

