#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using System.Collections.Generic;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration
{
    public class PrefabResourceLoaderRegistrar : ResourceLoaderRegistrar
    {
        public override IEnumerable<ResourceLoader> GetLoaders() =>
            ReflectionFactory.CreateAllInstances<PrefabResourceLoader>();
    }
}

#endif
