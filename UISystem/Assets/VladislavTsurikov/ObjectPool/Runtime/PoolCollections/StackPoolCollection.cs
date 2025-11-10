using System.Collections.Generic;

namespace VladislavTsurikov.ObjectPool.Runtime
{
    public class StackPoolCollection<T> : PoolCollection<T>
    {
        private readonly Stack<T> _stack;

        public StackPoolCollection() : this(10)
        {
        }

        public StackPoolCollection(int initialCapacity) => _stack = new Stack<T>(initialCapacity);

        public override int Count => _stack.Count;

        public override void Add(T item) => _stack.Push(item);

        public override T Remove() => _stack.Pop();

        public override bool Contains(T item) => _stack.Contains(item);

        public override void Clear() => _stack.Clear();
    }
}
