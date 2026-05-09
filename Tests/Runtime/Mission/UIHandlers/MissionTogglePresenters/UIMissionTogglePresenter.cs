#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using VladislavTsurikov.UISystem.Runtime.UnityUIIntegration;

namespace VladislavTsurikov.UISystem.Tests.Runtime.MissionTogglePresenters
{
    public abstract class UIMissionTogglePresenter : UnityUIPresenter
    {
        private UIMissionToggleView _view;

        protected abstract bool UnlockedTab { get; }

        protected abstract int NotificationCount { get; }

        protected abstract string ToggleBindingId { get; }

        protected override UniTask InitializeUIPresenter(
            CancellationToken cancellationToken,
            CompositeDisposable disposables)
        {
            _view = GetView<UIMissionToggleView>(ToggleBindingId);
            _view.SetActive(true);
            _view.SetActiveRedCircle(UnlockedTab);

            SetNotification(NotificationCount);

            InitializeUIMissionTogglePresenter(cancellationToken, disposables);

            _view.OnClicked
                .Subscribe(_ => OnToggleClicked(cancellationToken).Forget())
                .AddTo(disposables);

            return UniTask.CompletedTask;
        }

        protected virtual UniTask OnToggleClicked(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask InitializeUIMissionTogglePresenter(
            CancellationToken cancellationToken,
            CompositeDisposable disposables)
        {
            return UniTask.CompletedTask;
        }

        private void SetNotification(int count)
        {
            bool show = count > 0;
            _view.SetActiveRedCircle(show);
            _view.SetRedCircleAmount(count > 1 ? count.ToString() : "");
        }
    }
}

#endif

#endif
