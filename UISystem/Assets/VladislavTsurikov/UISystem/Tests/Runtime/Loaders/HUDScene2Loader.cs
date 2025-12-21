#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration.Attributes;

namespace VladislavTsurikov.UISystem.Tests.Runtime
{
    [SceneFilter("TestScene_2")]
    [PrefabAddress("HUDScene_2")]
    public class HUDScene2Loader : PrefabResourceLoader
    {
    }
}

#endif
