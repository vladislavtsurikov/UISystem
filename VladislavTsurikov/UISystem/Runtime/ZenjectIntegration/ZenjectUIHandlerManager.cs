using System;
using VladislavTsurikov.UISystem.Runtime.Core;
using Zenject;

namespace VladislavTsurikov.UISystem.Runtime
{
    public sealed class ZenjectUIHandlerManager : UIHandlerManager
    {
        private readonly DiContainer _container;

        public ZenjectUIHandlerManager(DiContainer container) => _container = container;

        protected override UIHandler CreateUIHandler(Type type) => (UIHandler)_container.Instantiate(type);

        protected override void RegisterInContainer(UIHandler handler)
        {
            var bindingId = UIHandlerBindingId.FromHandler(handler);

            _container.Bind(handler.GetType()).WithId(bindingId).FromInstance(handler).AsCached();
        }

        protected override void BeforeRemoveHandler(UIHandler handler)
        {
            Type type = handler.GetType();
            var bindingId = UIHandlerBindingId.FromHandler(handler);

            _container.UnbindId(type, bindingId);
        }
    }
}
