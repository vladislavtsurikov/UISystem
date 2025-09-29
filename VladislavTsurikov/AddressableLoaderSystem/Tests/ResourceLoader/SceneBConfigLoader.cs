using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.ZenjectIntegration;
using Zenject;

namespace VladislavTsurikov.AddressableLoaderSystem.Tests
{
    [SceneFilter("TestScene_B")]
    public class SceneBConfigLoader : BindableResourceLoader
    {
        public SceneBConfigLoader(DiContainer container) : base(container)
        {
        }

        public ConfigSceneB Config { get; private set; }
        public ConfigSceneBWithAssetReference ConfigWithReference { get; private set; }

        public override async UniTask LoadResourceLoader(CancellationToken token)
        {
            Config = await LoadAndBind<ConfigSceneB>(token, "ConfigSceneB");
            ConfigWithReference =
                await LoadAndBind<ConfigSceneBWithAssetReference>(token, "ConfigSceneB_WithAssetReference");
        }
    }
}
