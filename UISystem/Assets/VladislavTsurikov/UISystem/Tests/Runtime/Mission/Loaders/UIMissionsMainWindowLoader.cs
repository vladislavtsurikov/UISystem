#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Behavior;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration.Attributes;

namespace VladislavTsurikov.UISystem.Tests.Runtime
{
    [Behavior(typeof(SceneBehavior), "TestScene_1")]
    [PrefabAddress("MissionsMainWindow")]
    public class UIMissionsMainWindowLoader : PrefabResourceLoader
    {
    }
}

#endif
