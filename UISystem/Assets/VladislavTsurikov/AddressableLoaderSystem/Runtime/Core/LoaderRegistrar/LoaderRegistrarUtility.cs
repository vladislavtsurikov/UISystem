#if ADDRESSABLE_LOADER_SYSTEM_ADDRESSABLES
using System.Collections.Generic;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core
{
    public static class LoaderRegistrarUtility
    {
        internal static void RegisterLoaderInitializers(ResourceLoaderManager manager)
        {
            var resourceLoaderRegistrar = ReflectionFactory.CreateAllInstances<ResourceLoaderRegistrar>();

            foreach (ResourceLoaderRegistrar registrar in resourceLoaderRegistrar)
            {
                registrar.RegisterLoaders(manager);
            }
        }
    }
}
#endif
