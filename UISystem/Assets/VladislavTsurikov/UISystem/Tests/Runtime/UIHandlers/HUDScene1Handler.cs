#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UIRootSystem.Runtime.Layers;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UnityUIIntegration;
using Zenject;

namespace VladislavTsurikov.UISystem.Tests.Runtime
{
    [SceneFilter("TestScene_1")]
    [ParentUIHandler(typeof(HUD))]
    public class HUDScene1Handler : UnityUIHandler
    {
        public HUDScene1Handler(DiContainer container, HUDScene1Loader loader)
            : base(container, loader)
        {
        }

        protected override async UniTask InitializeUIHandler(CancellationToken cancellationToken,
            CompositeDisposable disposables) => await Show(cancellationToken);
    }
}

#endif

#endif

#endif
