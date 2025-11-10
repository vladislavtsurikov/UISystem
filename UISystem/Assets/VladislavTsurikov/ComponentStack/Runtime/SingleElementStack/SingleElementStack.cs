using System;
using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace Assemblies.VladislavTsurikov.ComponentStack.Runtime.SingleElementStack
{
    public interface ISingleElementStack
    {
        object GetObjectElement();
        void ReplaceElement(Type elementType);
    }

    public class SingleElementStack<T> : ComponentStack<T>, ISingleElementStack where T : Component
    {
        public T Value => _elementList.Count > 0 ? _elementList[0] : null;

        public void ReplaceElement(Type elementType)
        {
            RemoveAll();
            Create(elementType);
        }

        public object GetObjectElement() => GetElement();

        public override void OnRemoveInvalidElements()
        {
            if (_elementList.Count > 1)
            {
                RemoveAll();
            }
        }

        public void CreateFirstElementIfNecessary(Type elementType)
        {
            if (IsEmpty())
            {
                Create(elementType);
            }
        }

        public T GetElement() => _elementList.Count > 0 ? _elementList[0] : null;

        public bool IsEmpty() => _elementList.Count == 0;
    }
}
