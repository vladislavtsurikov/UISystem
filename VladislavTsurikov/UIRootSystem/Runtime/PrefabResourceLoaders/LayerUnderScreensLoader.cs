using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration;

namespace VladislavTsurikov.UIRootSystem.Runtime.PrefabResourceLoaders
{
    [SceneFilter("TestScene_1", "TestScene_2")]
    public class LayerUnderScreensLoader : PrefabResourceLoader
    {
        public override string PrefabAddress => "LayerUnderScreens";
    }
}
