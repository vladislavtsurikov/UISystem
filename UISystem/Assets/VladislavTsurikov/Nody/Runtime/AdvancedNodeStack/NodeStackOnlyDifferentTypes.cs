using System;
using Node = VladislavTsurikov.Nody.Runtime.Core.Node;

namespace VladislavTsurikov.Nody.Runtime.AdvancedNodeStack
{
    public class NodeStackOnlyDifferentTypes<T> : AdvancedNodeStack<T>
        where T : Node
    {
        public override void OnRemoveInvalidElements() => RemoveElementsWithSameType();

        public void SetupElement<T2>(bool force = false) where T2 : Node =>
            SetupElement(typeof(T2), force);

        public void SetupElement(Type type, bool force = false)
        {
            if (!typeof(T).IsAssignableFrom(type))
            {
                throw new ArgumentOutOfRangeException(nameof(type));
            }

            Node node = GetElement(type);

            node?.Setup(force);
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

        public T2 GetAndAutoUpdateNode<T2>(Action<T2> updateNode) where T2 : Node
        {
            T2 node = GetElement<T2>();

            if (node != null)
            {
                ListChanged += CollectionChanged;

                void CollectionChanged()
                {
                    T2 newNode = GetElement<T2>();
                    updateNode(newNode);

                    if (newNode == null)
                    {
                        ListChanged -= CollectionChanged;
                    }
                }
            }

            return node;
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
