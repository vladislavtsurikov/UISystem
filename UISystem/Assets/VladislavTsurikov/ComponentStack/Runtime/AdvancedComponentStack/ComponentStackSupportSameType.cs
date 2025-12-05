using System;
using System.Collections.Generic;
using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack
{
    public class ComponentStackSupportSameType<T> : AdvancedComponentStack<T>
        where T : Component
    {
        public void CreateComponentsIfMissingType(Type[] types) => CreateElementIfMissingType(types);

        public T CreateComponentIfMissingType(Type type) => CreateElementIfMissingType(type);

        public void CreateComponents(List<Type> types)
        {
            foreach (Type type in types)
            {
                Create(type);
            }
        }

        public T CreateComponent(Type type, int index = -1) => Create(type, index);

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
