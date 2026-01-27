using System;
using System.Collections.Generic;
using System.Linq;
using OdinSerializer.Utilities;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility.Runtime;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.Nody.Runtime.AdvancedNodeStack
{
    public abstract class AdvancedNodeStack<T> : NodeStack<T>
        where T : Node
    {
        /// <summary>
        /// A public link to the list is required for UnityEditorInternal.ReorderableList
        /// </summary>
        public ObservableList<T> List => _elementList;

        private protected override void CreateElements()
        {
            OnCreateElements();

            GetType().GetAttribute<CreateNodesAttribute>()?.Types
                .ForEach(type => CreateElementIfMissingType(type));

            AllTypesDerivedFrom<T>.Types
                .Where(type => type.GetAttribute<PersistentNodeAttribute>() != null)
                .ForEach(type => CreateElementIfMissingType(type));
        }

        protected virtual void OnCreateElements()
        {
        }

        public void CreateAllElementTypes() => CreateElementIfMissingType(AllTypesDerivedFrom<T>.Types);

        public void SyncToTypes(Type[] types)
        {
            RemoveInvalidElements();

            if (types == null)
            {
                return;
            }

            IsDirty = true;

            Dictionary<Type, Queue<T>> pool = new Dictionary<Type, Queue<T>>();

            for (int i = 0; i < _elementList.Count; i++)
            {
                T element = _elementList[i];
                Type elementType = element.GetType();

                if (!pool.TryGetValue(elementType, out Queue<T> queue))
                {
                    queue = new Queue<T>();
                    pool.Add(elementType, queue);
                }

                queue.Enqueue(element);
            }

            List<T> newList = new List<T>(types.Length);

            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];

                if (pool.TryGetValue(type, out Queue<T> queue) && queue.Count > 0)
                {
                    newList.Add(queue.Dequeue());
                    continue;
                }

                int beforeCount = _elementList.Count;

                Create(type);

                int afterCount = _elementList.Count;
                if (afterCount > beforeCount)
                {
                    newList.Add(_elementList[afterCount - 1]);
                }
            }

            HashSet<T> keep = new HashSet<T>(newList);

            for (int i = _elementList.Count - 1; i >= 0; i--)
            {
                if (!keep.Contains(_elementList[i]))
                {
                    Remove(i);
                }
            }

            _elementList.Clear();
            _elementList.AddRange(newList);
        }

        public T2 GetElement<T2>() where T2 : Node
        {
            object node = GetElement(typeof(T2), out _);

            if (node == null)
            {
                return null;
            }

            return (T2)node;
        }

        protected void CreateElementIfMissingType(Type[] types)
        {
            foreach (Type type in types)
            {
                CreateElementIfMissingType(type);
            }
        }

        protected T CreateElementIfMissingType(Type type)
        {
            T settings = GetElement(type);
            if (settings == null)
            {
                return Create(type);
            }

            return settings;
        }

        protected void AddElementIfMissingType(T element)
        {
            T settings = GetElement(element.GetType());

            if (settings == null)
            {
                Add(element);
            }
        }
    }
}
