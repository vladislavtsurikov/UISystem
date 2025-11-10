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
    [ParentUIHandler(typeof(HUDScene1Handler))]
    public class UIMissionsHUDButtonHandler : ButtonUIHandler
    {
        private MissionsHUDButtonView _view;

        public UIMissionsHUDButtonHandler(DiContainer container) : base(container)
        {
        }

        protected override UniTask InitializeUIHandler(CancellationToken cancellationToken,
            CompositeDisposable disposables)
        {
            _view = GetUIComponent<MissionsHUDButtonView>("MissionsHUDButtonView");

            _view.OnClicked
                .Subscribe(async _ =>
                {
                    await UINavigator.Show<UIMissionsMainWindowHandler, Screens>(cancellationToken);
                })
                .AddTo(disposables);
            return UniTask.CompletedTask;
        }
    }
}

#endif

#endif

#endif
