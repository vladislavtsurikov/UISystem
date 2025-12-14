#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration;

namespace VladislavTsurikov.UIRootSystem.Runtime.PrefabResourceLoaders
{
    [SceneFilter("TestScene_1", "TestScene_2")]
    public class PopupsLoader : PrefabResourceLoader
    {
        public override string PrefabAddress => "Popups";
    }
}

#endif
