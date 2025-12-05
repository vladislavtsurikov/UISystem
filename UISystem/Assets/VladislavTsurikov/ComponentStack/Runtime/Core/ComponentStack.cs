using System;
using System.Collections.Generic;
using System.Linq;
 
using OdinSerializer;
using OdinSerializer.Utilities;
using VladislavTsurikov.AttributeUtility.Runtime;

namespace VladislavTsurikov.ComponentStack.Runtime.Core
{
    [Serializable]
    public abstract class ComponentStack<T> where T : Component
    {
        [OdinSerialize]
        protected AdvancedElementList<T> _elementList = new();

        [NonSerialized]
        public bool IsDirty = true;

        public object[] SetupData { get; private set; }

        public IReadOnlyList<T> ElementList => _elementList;

        public bool IsSetup { get; private set; }

        public T SelectedElement => _elementList.FirstOrDefault(t => t.Selected);

        public event Action<int> ElementAdded;
        public event Action<int> ElementRemoved;
        public event Action ListChanged;

        public void Setup(bool force = true, object[] setupData = null)
        {
            _elementList ??= new AdvancedElementList<T>();

            _elementList.OnAdded += HandleElementAdded;
            _elementList.OnRemoved += HandleElementRemoved;
            _elementList.OnListChanged += HandleListChanged;

            IsSetup = true;

            SetupData = setupData;

            OnSetup();
            RemoveInvalidElements();
            CreateElements();

            foreach (T element in _elementList)
            {
                element.Stack = this;
                element.SetupWithSetupData(force, setupData);
            }

            IsDirty = true;
        }

        public void OnDisable()
        {
            IsSetup = false;

            for (var i = 0; i < _elementList.Count; i++)
            {
                ((IDisableable)_elementList[i]).OnDisable();
            }

            _elementList.OnAdded -= HandleElementAdded;
            _elementList.OnRemoved -= HandleElementRemoved;
            _elementList.OnListChanged -= HandleListChanged;

            OnDisableStack();
        }

        public void Clear()
        {
            for (var i = _elementList.Count - 1; i >= 0; i--)
            {
                if (Remove(i))
                {
                    IsDirty = true;
                }
            }
        }

        protected T Create(Type type, int index = -1)
        {
            if (!AllowCreate(type))
            {
                return null;
            }

            if (type.GetAttribute(typeof(DontCreateAttribute)) != null)
            {
                return null;
            }

            T element = Instantiate(type, false);
            Add(element, index);
            element.Stack = this;
            element.SetupWithSetupData(true, SetupData);
            element.OnCreateInternal();

            return element;
        }

        public T Instantiate(Type type, bool setup = true)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (typeof(T).IsAssignableFrom(type))
            {
                var element = (T)Activator.CreateInstance(type);

                if (setup)
                {
                    element.SetupWithSetupData(true, SetupData);
                    element.OnCreateInternal();
                }

                return element;
            }

            throw new ArgumentOutOfRangeException(nameof(type));
        }

        protected void Add(T element, int index = -1)
        {
            if (element == null)
            {
                return;
            }

            if (!AllowCreate(element.GetType()))
            {
                return;
            }

            if (index == -1)
            {
                _elementList.Add(element);
            }
            else
            {
                _elementList.Insert(index, element);
            }

            if (_elementList.Count == 1)
            {
                element.Selected = true;
            }

            IsDirty = true;
        }

        public void Reset()
        {
            for (var i = 0; i < ElementList.Count; i++)
            {
                Reset(i);
            }
        }

        public void Reset(int index)
        {
            T oldElement = _elementList[index];
            oldElement.IsHappenedReset = true;

            T newElement = Create(oldElement.GetType(), index);
            newElement.Stack = this;

            newElement.OnReset(oldElement);

            IsDirty = true;
        }

        public void RemoveAll()
        {
            for (var i = _elementList.Count - 1; i >= 0; i--)
            {
                Remove(i);
            }
        }

        public bool Remove(int index)
        {
            T element = _elementList[index];

            if (element == null || element.IsDeletable())
            {
                _elementList.RemoveAt(index);
                IsDirty = true;
                return true;
            }

            return false;
        }

        public void ReplaceElement(T element, int index)
        {
            if (!Remove(index))
            {
                return;
            }

            _elementList.Insert(index, element);
            IsDirty = true;
        }

        public void Select(T element)
        {
            _elementList.ForEach(setting => setting.Selected = false);
            element.Selected = true;
        }

        public void RemoveInvalidElements()
        {
            for (var i = _elementList.Count - 1; i >= 0; i--)
            {
                if (_elementList[i] == null || _elementList[i].GetType().IsAbstract || !_elementList[i].DeleteElement())
                {
                    Remove(i);
                }
            }

            OnRemoveInvalidElements();
        }

        public List<T> FindAll(Predicate<T> match)
        {
            if (match == null)
            {
                return new List<T>();
            }

            var all = new List<T>();

            foreach (T element in _elementList)
            {
                if (match(element))
                {
                    all.Add(element);
                }
            }

            return all;
        }

        public T GetElement(Type type) => GetElement(type, out _);

        public T GetElement(Type type, out int index)
        {
            index = -1;
            for (var i = 0; i < _elementList.Count; i++)
            {
                if (_elementList[i] != null)
                {
                    if (_elementList[i].GetType() == type)
                    {
                        index = i;
                        return _elementList[i];
                    }
                }
            }

            return null;
        }

        private void HandleElementAdded(int index) => ElementAdded?.Invoke(index);

        private void HandleElementRemoved(int index) => ElementRemoved?.Invoke(index);

        private void HandleListChanged() => ListChanged?.Invoke();

        protected virtual void OnSetup()
        {
        }

        protected virtual bool AllowCreate(Type type) => true;

        private protected virtual void CreateElements()
        {
        }

        protected virtual void OnDisableStack()
        {
        }

        public virtual void OnRemoveInvalidElements()
        {
        }
    }
}
