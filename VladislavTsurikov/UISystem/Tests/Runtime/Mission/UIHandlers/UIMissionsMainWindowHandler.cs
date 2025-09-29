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
    [ParentUIHandler(typeof(Screens))]
    public class UIMissionsMainWindowHandler : UnityUIHandler
    {
        public UIMissionsMainWindowHandler(DiContainer container, UIMissionsMainWindowLoader loader)
            : base(container, loader)
        {
        }

        protected override bool AllowMultipleActiveChildren => false;

        public MainMissionsWindowView View { get; private set; }

        protected override UniTask AfterShowUIHandler(CancellationToken ct, CompositeDisposable disposables)
        {
            if (View == null)
            {
                View = GetUIComponent<MainMissionsWindowView>("MainMissionsWindowView");

                View.OnCloseClicked
                    .Subscribe(_ => Hide(ct).Forget())
                    .AddTo(disposables);
            }

            return UniTask.CompletedTask;
        }
    }
}
