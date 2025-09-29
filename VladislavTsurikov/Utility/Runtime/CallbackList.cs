using System;
using System.Collections;
using System.Collections.Generic;
using OdinSerializer;

namespace VladislavTsurikov.Utility.Runtime
{
    [Serializable]
    public class CallbackList<T> :
        IList<T>,
        IList,
        IReadOnlyList<T>
    {
        [OdinSerialize]
        private List<T> _list = new();

        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (T)value;
        }

        public bool IsFixedSize => false;

        public bool IsSynchronized => false;

        public object SyncRoot => null;

        int IList.Add(object value)
        {
            Add((T)value);
            return _list.Count - 1;
        }

        void IList.Remove(object value) => Remove((T)value);

        bool IList.Contains(object value) => Contains((T)value);

        void ICollection.CopyTo(Array array, int index) => CopyTo((T[])array, index);

        int IList.IndexOf(object value) => IndexOf((T)value);

        void IList.Insert(int index, object value) => Insert(index, (T)value);

        public T this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            if (item == null)
            {
                return;
            }

            _list.Add(item);
            OnAdded?.Invoke(_list.Count - 1);
            OnListChanged?.Invoke();
        }

        public void Clear()
        {
            for (var i = _list.Count - 1; i >= 0; i--)
            {
                RemoveAt(i);
            }
        }

        public bool Remove(T item)
        {
            if (item == null)
            {
                return false;
            }

            var index = _list.IndexOf(item);

            RemoveAt(index);

            return index != -1;
        }

        public void RemoveAt(int index)
        {
            if (_list[index] != null)
            {
                OnRemoved?.Invoke(index);
                OnListChanged?.Invoke();
            }

            _list.RemoveAt(index);
        }

        public bool Contains(T item) => _list.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

        public int IndexOf(T item) => _list.IndexOf(item);

        public void Insert(int index, T item)
        {
            if (item == null)
            {
                return;
            }

            _list.Insert(index, item);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public event Action<int> OnAdded;
        public event Action<int> OnRemoved;
        public event Action OnListChanged;

        public void RemoveAll(Predicate<T> match)
        {
            for (var i = _list.Count - 1; i >= 0; i--)
            {
                if (match(_list[i]))
                {
                    RemoveAt(i);
                }
            }
        }

        public List<T> FindAll(Predicate<T> match) => _list.FindAll(match);
    }
}
