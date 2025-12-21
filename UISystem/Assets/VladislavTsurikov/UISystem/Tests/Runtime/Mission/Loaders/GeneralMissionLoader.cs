#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration.Attributes;

namespace VladislavTsurikov.UISystem.Tests.Runtime
{
    [SceneFilter("TestScene_1")]
    [PrefabAddress("GeneralMissions")]
    public class GeneralMissionLoader : PrefabResourceLoader
    {
    }
}

#endif
