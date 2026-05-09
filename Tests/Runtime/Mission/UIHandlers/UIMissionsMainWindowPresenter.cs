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

namespace VladislavTsurikov.UISystem.Tests.Runtime
{
    [SceneFilter("TestScene_1")]
    [UIParent(typeof(Screens))]
    public class UIMissionsMainWindowPresenter : UnityUIPresenter
    {
        private readonly SingleActiveChildPresenterModule _singleActiveChildPresenterModule;

        public MainMissionsWindowView View { get; private set; }

        public UIMissionsMainWindowPresenter(UIMissionsMainWindowLoader loader)
            : base(loader) =>
            _singleActiveChildPresenterModule = new SingleActiveChildPresenterModule(this);

        protected override UniTask InitializeUIPresenter(
            CancellationToken cancellationToken,
            CompositeDisposable disposables)
        {
            _singleActiveChildPresenterModule.Initialize(disposables);

            return UniTask.CompletedTask;
        }

        protected override UniTask AfterShowUIPresenter(CancellationToken ct, CompositeDisposable disposables)
        {
            if (View != null)
            {
                return UniTask.CompletedTask;
            }

            View = GetView<MainMissionsWindowView>("MainMissionsWindowView");
            View.OnCloseClicked
                .Subscribe(_ => Hide(ct).Forget())
                .AddTo(disposables);

            return UniTask.CompletedTask;
        }
    }
}

#endif

#endif

#endif
