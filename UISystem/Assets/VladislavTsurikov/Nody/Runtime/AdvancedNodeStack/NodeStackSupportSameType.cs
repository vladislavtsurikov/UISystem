using System;
using System.Collections.Generic;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.Nody.Runtime.AdvancedNodeStack
{
    public class NodeStackSupportSameType<T> : AdvancedNodeStack<T>
        where T : Node
    {
        public void CreateNodesIfMissingType(Type[] types) => CreateElementIfMissingType(types);

        public T CreateNodeIfMissingType(Type type) => CreateElementIfMissingType(type);

        public void CreateNodes(List<Type> types)
        {
            foreach (Type type in types)
            {
                Create(type);
            }
        }

        public T CreateNode(Type type, int index = -1) => Create(type, index);

        public List<T> GetElementsOfType(Type type)
        {
            var elements = new List<T>();
            for (var i = 0; i < _elementList.Count; i++)
            {
                if (_elementList[i] != null && _elementList[i].GetType() == type)
                {
                    elements.Add(_elementList[i]);
                }
            }

            return elements;
        }
    }
}
