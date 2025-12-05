using System;

namespace VladislavTsurikov.ObjectPool.Runtime
{
    public abstract class ObjectPool<T> where T : class
    {
        private readonly bool _collectionCheck;
        private readonly int _maxSize;
        private readonly PoolCollection<T> _poolCollection;

        protected ObjectPool(PoolCollection<T> poolCollection, int maxSize = 10000, bool collectionCheck = false)
        {
            if (maxSize <= 0)
            {
                throw new ArgumentException("Max size must be greater than 0", nameof(maxSize));
            }

            _poolCollection = poolCollection ?? throw new ArgumentNullException(nameof(poolCollection));
            _maxSize = maxSize;
            _collectionCheck = collectionCheck;
        }

        public int CountAll { get; private set; }
        public int CountActive => CountAll - CountInactive;
        public int CountInactive => _poolCollection.Count;

        protected abstract T CreateInstance();

        protected virtual void OnGet(T obj)
        {
        }

        protected virtual void OnRelease(T obj)
        {
        }

        protected virtual void OnDestroy(T obj)
        {
        }

        public T Get()
        {
            T item;
            if (_poolCollection.Count == 0)
            {
                item = CreateInstance();
                CountAll++;
            }
            else
            {
                item = _poolCollection.Remove();
            }

            OnGet(item);
            return item;
        }

        public void Release(T item)
        {
            if (_collectionCheck && _poolCollection.Contains(item))
            {
                throw new InvalidOperationException("Trying to release an object that is already in the pool.");
            }

            OnRelease(item);

            if (_poolCollection.Count < _maxSize)
            {
                _poolCollection.Add(item);
            }
            else
            {
                OnDestroy(item);
            }
        }

        public void Clear()
        {
            _poolCollection.Clear();
            CountAll = 0;
        }
    }
}
