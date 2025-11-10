#if ADDRESSABLE_LOADER_SYSTEM_ADDRESSABLES
using System.Collections.Generic;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core
{
    public abstract class ResourceLoaderRegistrar
    {
        public abstract IEnumerable<ResourceLoader> GetLoaders();

        public void RegisterLoaders(ResourceLoaderManager manager)
        {
            foreach (ResourceLoader loader in GetLoaders())
            {
                manager.Register(loader);
            }
        }
    }
}
#endif
