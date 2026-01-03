using System;
using System.Collections;
using System.Collections.Generic;
using OdinSerializer;
using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.Utility.Runtime
{
    [Serializable]
    public class ObservableList<T> :
        IList<T>,
        IList,
        IReadOnlyList<T>
    {
        [OdinSerialize]
        private List<T> _list = new List<T>();

        public T this[int index] { get => _list[index]; set => _list[index] = value; }

        object IList.this[int index] { get => this[index]; set => this[index] = (T)value; }

        public int Count => _list.Count;

        public bool IsFixedSize => false;

        public bool IsReadOnly => false;

        public bool IsSynchronized => false;

        public object SyncRoot => null;

        public event Action<int> OnAdded;
        public event Action<int> OnRemoved;
        public event Action OnListChanged;

        int IList.Add(object value)
        {
            Add((T)value);
            return _list.Count - 1;
        }

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
            for (int i = _list.Count - 1; i >= 0; i--)
            {
                RemoveAt(i);
            }
        }

        void IList.Remove(object value)
        {
            Remove((T)value);
        }

        public bool Remove(T item)
        {
            if (item == null)
            {
                return false;
            }

            int index = _list.IndexOf(item);

            if (index == -1)
            {
                return false;
            }

            RemoveAt(index);

            return true;
        }

        public void RemoveAll(Predicate<T> match)
        {
            for (int i = _list.Count - 1; i >= 0; i--)
            {
                if (match(_list[i]))
                {
                    RemoveAt(i);
                }
            }
        }

        public void RemoveAt(int index)
        {
            T item = _list[index];
            if (item == null)
            {
                _list.RemoveAt(index);
                return;
            }

            OnRemoved?.Invoke(index);

            if (item is IRemovable removable)
            {
                removable.OnRemove();
            }

            OnListChanged?.Invoke();

            _list.RemoveAt(index);
        }

        bool IList.Contains(object value)
        {
            return Contains((T)value);
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((T[])array, index);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((T)value);
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        public void Insert(int index, T item)
        {
            if (item == null)
            {
                return;
            }

            _list.Insert(index, item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public List<T> FindAll(Predicate<T> match)
        {
            return _list.FindAll(match);
        }

        public void AddRange(List<T> newList)
        {
            foreach (var item in newList)
            {
                _list.Add(item);
                OnAdded?.Invoke(_list.Count - 1);
            }

            OnListChanged?.Invoke();
        }
    }
}
