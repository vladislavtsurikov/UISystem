using System;
using UnityEngine;
using VladislavTsurikov.UISystem.Runtime.UnityUIIntegration.Utility;
using Zenject;

namespace VladislavTsurikov.UISystem.Runtime.UnityUIIntegration
{
    public abstract class ComponentBindingUIHandler : DiContainerUIHandler
    {
        protected readonly UIComponentBinder ComponentBinder;

        protected ComponentBindingUIHandler(DiContainer container)
            : base(container) =>
            ComponentBinder = new UIComponentBinder(container, this);

        public T GetUIComponent<T>(string bindingId, Type handlerType, int index = 0) where T : MonoBehaviour =>
            ResolveWithId<T>(bindingId, handlerType, index);

        public virtual T GetUIComponent<T>(string bindingId, int index = 0) where T : MonoBehaviour =>
            ResolveWithId<T>(bindingId, GetType(), index);

        protected T ResolveWithId<T>(string bindingId, Type handlerType, int index) where T : MonoBehaviour
        {
            var id = UIBindingId.FromTypeAndIndex(handlerType, bindingId, index);
            return _container.ResolveId<T>(id);
        }

        protected virtual void DisposeComponentBindingUIHandler()
        {
        }

        public override void DisposeUIHandler()
        {
            ComponentBinder.Dispose();
            DisposeComponentBindingUIHandler();
        }
    }
}
