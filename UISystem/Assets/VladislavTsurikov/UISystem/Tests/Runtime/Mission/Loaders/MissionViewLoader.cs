#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration;

namespace VladislavTsurikov.UISystem.Tests.Runtime
{
    [SceneFilter("TestScene_1")]
    public class MissionViewLoader : PrefabResourceLoader
    {
        [AutoLoad("MissionView")]
        public override string PrefabAddress => "MissionView";
    }
}

#endif
