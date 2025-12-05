#if ADDRESSABLE_LOADER_SYSTEM_VCONTAINER
using UnityEngine;
using VContainer;
using VContainer.Unity;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.VContainerIntegration
{
    public class AddressableLoaderLifetimeScope : LifetimeScope
    {
        internal static AddressableLoaderLifetimeScope Instance;

        protected override void Awake()
        {
            Instance = this;
            Object.DontDestroyOnLoad(gameObject);
        }

        protected override void Configure(IContainerBuilder builder)
        {
            var initializer = new VContainerAddressableLoaderSystemInitializer(builder);
            initializer.InitializeSystem();

            builder.RegisterInstance(initializer)
                .As<IStartable>()
                .AsSelf();
        }
    }
}
#endif
