#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration;

namespace VladislavTsurikov.UISystem.Tests.Runtime
{
    [SceneFilter("TestScene_1")]
    public class HUDScene1Loader : PrefabResourceLoader
    {
        public override string PrefabAddress => "HUDScene_1";
    }
}

#endif
