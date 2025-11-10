#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UIRootSystem.Runtime.PrefabResourceLoaders;
using VladislavTsurikov.UISystem.Runtime.Core;
using Zenject;

namespace VladislavTsurikov.UIRootSystem.Runtime.Layers
{
    [ParentUIHandler(typeof(UIRoot))]
    [SceneFilter("TestScene_1", "TestScene_2")]
    public class Screens : UILayer
    {
        public Screens(DiContainer container, ScreensLoader loader)
            : base(container, loader)
        {
        }

        public override int GetLayerIndex() => 2;

        protected override async UniTask InitializeUIHandler(CancellationToken cancellationToken,
            CompositeDisposable disposables) => await Show(cancellationToken);
    }
}

#endif

#endif

#endif
