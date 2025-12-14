#if ADDRESSABLE_LOADER_SYSTEM_ADDRESSABLES
#if ADDRESSABLE_LOADER_SYSTEM_ZENJECT
using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.ZenjectIntegration;
using Zenject;

namespace VladislavTsurikov.AddressableLoaderSystem.Tests
{
    [SceneFilter("TestScene_B")]
    public class SceneBConfigLoader : ZenjectResourceLoader
    {
        public SceneBConfigLoader(DiContainer container) : base(container)
        {
        }

        public ConfigSceneB ConfigSceneB { get; private set; }

        public ConfigSceneBWithAssetReference ConfigSceneB_WithAssetReference { get; private set; }

        public override async UniTask LoadResourceLoader(CancellationToken token)
        {
            ConfigSceneB = await LoadAndBind<ConfigSceneB>(token, "ConfigSceneB");
            ConfigSceneB_WithAssetReference =
                await LoadAndBind<ConfigSceneBWithAssetReference>(token, "ConfigSceneB_WithAssetReference");
        }
    }
}
#endif
#endif
