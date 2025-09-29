using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration;

namespace VladislavTsurikov.UISystem.Tests.Runtime
{
    [SceneFilter("TestScene_1")]
    public class GeneralMissionLoader : PrefabResourceLoader
    {
        public override string PrefabAddress => "GeneralMissions";
    }
}
