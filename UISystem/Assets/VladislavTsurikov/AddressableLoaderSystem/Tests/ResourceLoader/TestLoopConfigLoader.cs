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
    public class TestLoopConfigLoader : ZenjectResourceLoader
    {
        public TestLoopConfigLoader(DiContainer container) : base(container)
        {
        }

        public TestLoopConfig TestLoopConfig { get; private set; }

        public override async UniTask LoadResourceLoader(CancellationToken token) =>
            TestLoopConfig = await LoadAndBind<TestLoopConfig>(token, "TestLoopConfig");
    }
}
#endif
#endif
