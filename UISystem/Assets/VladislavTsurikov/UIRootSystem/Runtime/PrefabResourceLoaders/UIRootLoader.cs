#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration.Attributes;

namespace VladislavTsurikov.UIRootSystem.Runtime.PrefabResourceLoaders
{
    [SceneFilter("TestScene_1", "TestScene_2")]
    [PrefabAddress("UI Root")]
    public class UIRootLoader : PrefabResourceLoader
    {
    }
}

#endif
