using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration;

namespace VladislavTsurikov.UISystem.Tests.Runtime
{
    [SceneFilter("TestScene_2")]
    public class HUDScene2Loader : PrefabResourceLoader
    {
        public override string PrefabAddress => "HUDScene_2";
    }
}
