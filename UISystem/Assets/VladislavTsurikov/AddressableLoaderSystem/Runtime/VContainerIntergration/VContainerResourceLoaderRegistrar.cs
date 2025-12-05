#if ADDRESSABLE_LOADER_SYSTEM_VCONTAINER
using System;
using System.Collections.Generic;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.VContainerIntegration;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.VContainerIntergration
{
    public class VContainerResourceLoaderRegistrar : ResourceLoaderRegistrar
    {
        public override IEnumerable<ResourceLoader> GetLoaders()
        {
            AddressableLoaderLifetimeScope scope = AddressableLoaderLifetimeScope.Instance;
            if (scope == null)
            {
                throw new InvalidOperationException("AddressableLoaderLifetimeScope is not initialized.");
            }

            IObjectResolver resolver = scope.Container;
            return ReflectionFactory.CreateAllInstances<VContainerResourceLoader>(resolver);
        }
    }
}
#endif
