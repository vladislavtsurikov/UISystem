using System;
using Runtime_Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack
{
    /// <summary>
    ///     Prevents elements of the same type from being created.
    /// </summary>
    public class ComponentStackOnlyDifferentTypes<T> : AdvancedComponentStack<T>
        where T : Runtime_Core_Component
    {
        public override void OnRemoveInvalidElements() => RemoveElementsWithSameType();

        public void SetupElement<T2>(bool force = false) where T2 : Runtime_Core_Component =>
            SetupElement(typeof(T2), force);

        public T2 GetElement<T2>() where T2 : Runtime_Core_Component
        {
            object component = GetElement(typeof(T2), out _);

            if (component == null)
            {
                return null;
            }

            return (T2)component;
        }

        public void SetupElement(Type type, bool force = false)
        {
            if (!typeof(T).IsAssignableFrom(type))
            {
                throw new ArgumentOutOfRangeException(nameof(type));
            }

            Runtime_Core_Component component = GetElement(type);

            component?.Setup(force);
        }

        public void CreateIfMissingType(Type[] types) => CreateElementIfMissingType(types);

        public T CreateIfMissingType(Type type) => CreateElementIfMissingType(type);

        public void AddIfMissingType(T element) => AddElementIfMissingType(element);

        public bool Remove(Type type)
        {
            GetElement(type, out var index);

            if (index != -1)
            {
                return Remove(index);
            }

            return false;
        }

        public void ReplaceElement(T element)
        {
            for (var i = 0; i < _elementList.Count; i++)
            {
                if (_elementList[i].GetType() == element.GetType())
                {
                    _elementList[i] = element;
                    IsDirty = true;
                    return;
                }
            }

            _elementList.Add(element);
            IsDirty = true;
        }

        public void Reset(Type type)
        {
            for (var i = 0; i < _elementList.Count; i++)
            {
                if (_elementList[i].GetType() == type)
                {
                    Reset(i);
                }
            }
        }

        public T2 GetAndAutoUpdateComponent<T2>(Action<T2> updateComponent) where T2 : Runtime_Core_Component
        {
            T2 component = GetElement<T2>();

            if (component != null)
            {
                ListChanged += CollectionChanged;

                void CollectionChanged()
                {
                    T2 newComponent = GetElement<T2>();
                    updateComponent(newComponent);

                    if (newComponent == null)
                    {
                        ListChanged -= CollectionChanged;
                    }
                }
            }

            return component;
        }

        public bool HasType(Type type) => GetElement(type) != null;

        protected bool HasMultipleType(Type type)
        {
            var count = 0;

            foreach (T element in _elementList)
            {
                if (element != null)
                {
                    if (element.GetType().ToString() == type.ToString())
                    {
                        count++;
                    }

                    if (count == 2)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        protected void RemoveElementsWithSameType()
        {
            for (var i = _elementList.Count - 1; i >= 0; i--)
            {
                if (HasMultipleType(_elementList[i].GetType()))
                {
                    Remove(i);
                }
            }
        }
    }
}
