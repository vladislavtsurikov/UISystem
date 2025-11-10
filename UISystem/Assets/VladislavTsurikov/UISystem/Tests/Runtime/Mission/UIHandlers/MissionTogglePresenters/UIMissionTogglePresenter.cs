#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using VladislavTsurikov.UISystem.Runtime.UnityUIIntegration;
using Zenject;

namespace VladislavTsurikov.UISystem.Tests.Runtime.MissionTogglePresenters
{
    public abstract class UIMissionTogglePresenter : ButtonUIHandler
    {
        private UIMissionToggleView _view;

        protected UIMissionTogglePresenter(DiContainer container)
            : base(container)
        {
        }

        protected abstract bool UnlockedTab { get; }

        protected abstract int NotificationCount { get; }

        protected abstract UIMissionToggleView ResolveView();

        protected override UniTask InitializeUIHandler(CancellationToken cancellationToken,
            CompositeDisposable disposables)
        {
            _view = ResolveView();
            _view.SetActive(true);
            _view.SetActiveRedCircle(UnlockedTab);

            SetNotification(NotificationCount);

            InitializeUIMissionTogglePresenter(cancellationToken, disposables);

            _view.OnClicked
                .Subscribe(_ => OnToggleClicked(cancellationToken).Forget())
                .AddTo(disposables);

            return UniTask.CompletedTask;
        }

        protected virtual UniTask OnToggleClicked(CancellationToken cancellationToken) => UniTask.CompletedTask;

        protected virtual UniTask InitializeUIMissionTogglePresenter(CancellationToken cancellationToken,
            CompositeDisposable disposables) => UniTask.CompletedTask;

        private void SetNotification(int count)
        {
            var show = count > 0;
            _view.SetActiveRedCircle(show);

            if (count > 1)
            {
                _view.SetRedCircleAmount(count.ToString());
            }
            else
            {
                _view.SetRedCircleAmount("");
            }
        }
    }
}

#endif

#endif
