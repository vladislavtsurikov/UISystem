#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UnityUIIntegration;

namespace VladislavTsurikov.UISystem.Tests.Runtime
{
    [SceneFilter("TestScene_1")]
    [UIParent(typeof(HUDScene1Presenter))]
    public class UIMissionsHUDButtonPresenter : UnityUIPresenter
    {
        protected override UniTask InitializeUIPresenter(
            CancellationToken cancellationToken,
            CompositeDisposable disposables)
        {
            MissionsHUDButtonView view = GetView<MissionsHUDButtonView>("MissionsHUDButtonView");

            view.OnClicked
                .Subscribe(_ => UINavigator.Show<UIMissionsMainWindowPresenter>(cancellationToken).Forget())
                .AddTo(disposables);

            return UniTask.CompletedTask;
        }
    }
}

#endif

#endif

#endif
