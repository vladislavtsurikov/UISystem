#if ADDRESSABLE_LOADER_SYSTEM_ZENJECT
using System.Collections.Generic;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.ReflectionUtility.Runtime;
using Zenject;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.ZenjectIntegration
{
    public class ZenjectResourceLoaderRegistrar : ResourceLoaderRegistrar
    {
        public override IEnumerable<ResourceLoader> GetLoaders()
        {
            DiContainer container = ProjectContext.Instance.Container;
            return ReflectionFactory.CreateAllInstances<ZenjectResourceLoader>(container);
        }
    }
}
#endif
