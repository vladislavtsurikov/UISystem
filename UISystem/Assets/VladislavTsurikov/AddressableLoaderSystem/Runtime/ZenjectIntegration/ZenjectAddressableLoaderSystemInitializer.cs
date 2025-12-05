#if ADDRESSABLE_LOADER_SYSTEM_ADDRESSABLES
using System;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using Zenject;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.ZenjectIntegration
{
    public class ZenjectAddressableLoaderSystemInitializer : AddressableLoaderSystemInitializer, IInitializable
    {
        private readonly DiContainer _container;

        public ZenjectAddressableLoaderSystemInitializer(DiContainer container)
        {
            _container = container;
        }

        protected override void Bind(Type type, object instance)
        {
            _container.Bind(type).FromInstance(instance).AsSingle();
        }

        public void Initialize()
        {
            InitializeSystem();
        }
    }
}
#endif
