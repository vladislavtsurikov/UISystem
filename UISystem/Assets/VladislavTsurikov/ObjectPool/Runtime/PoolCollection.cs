namespace VladislavTsurikov.ObjectPool.Runtime
{
    public abstract class PoolCollection<T>
    {
        public abstract int Count { get; }

        public abstract void Add(T item);
        public abstract T Remove();
        public abstract bool Contains(T item);
        public abstract void Clear();
    }
}
