#if ADDRESSABLE_LOADER_SYSTEM_ADDRESSABLES
using System;
using UnityEngine;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core
{
    public abstract class AddressableLoaderSystemInitializer
    {
        public void InitializeSystem()
        {
            var config = AddressableLoaderSystemConfig.Instance;

            if (config == null)
            {
                Debug.LogError("[AddressableLoaderSystemInitializer] AddressableLoaderSystemConfig.Instance is missing.");
                return;
            }

            BindLoaders(config);
            BindBehaviors(config);

#if ADDRESSABLE_LOADER_LOGS
            Debug.Log("[AddressableLoaderSystemInitializer] AddressableLoaderSystem successfully initialized.");
#endif
        }

        protected abstract void Bind(Type type, object instance);

        protected virtual void BindLoaders(AddressableLoaderSystemConfig config)
        {
            foreach (var loader in config.GetAllLoaders())
            {
                if (loader == null)
                {
                    Debug.LogError("[AddressableLoaderSystemInitializer] Encountered null ResourceLoader during binding.");
                    continue;
                }

                var type = loader.GetType();
                Bind(type, loader);
            }
        }

        protected virtual void BindBehaviors(AddressableLoaderSystemConfig config)
        {
            foreach (var behavior in config.GetAllBehaviors())
            {
                if (behavior == null)
                {
                    Debug.LogError("[AddressableLoaderSystemInitializer] Encountered null LoaderBehavior during binding.");
                    continue;
                }

                var type = behavior.GetType();
                Bind(type, behavior);
            }
        }
    }
}
#endif
