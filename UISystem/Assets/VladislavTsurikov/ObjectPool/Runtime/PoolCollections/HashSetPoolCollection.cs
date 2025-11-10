using System.Collections.Generic;
using System.Linq;

namespace VladislavTsurikov.ObjectPool.Runtime
{
    public class HashSetPoolCollection<T> : PoolCollection<T> where T : class
    {
        private readonly HashSet<T> _hashSet;

        public HashSetPoolCollection() : this(10)
        {
        }

        public HashSetPoolCollection(int initialCapacity) => _hashSet = new HashSet<T>(initialCapacity);

        public override int Count => _hashSet.Count;

        public override void Add(T item) => _hashSet.Add(item);

        public override T Remove()
        {
            T item = _hashSet.First();
            _hashSet.Remove(item);
            return item;
        }

        public override bool Contains(T item) => _hashSet.Contains(item);

        public override void Clear() => _hashSet.Clear();
    }
}
