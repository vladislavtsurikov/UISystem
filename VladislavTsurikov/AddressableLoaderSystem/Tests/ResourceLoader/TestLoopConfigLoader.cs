using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.ZenjectIntegration;
using Zenject;

namespace VladislavTsurikov.AddressableLoaderSystem.Tests
{
    [SceneFilter("TestScene_B")]
    public class TestLoopConfigLoader : BindableResourceLoader
    {
        public TestLoopConfigLoader(DiContainer container) : base(container)
        {
        }

        public TestLoopConfig Config { get; private set; }

        public override async UniTask LoadResourceLoader(CancellationToken token) =>
            Config = await LoadAndBind<TestLoopConfig>(token, "TestLoopConfig");
    }
}
