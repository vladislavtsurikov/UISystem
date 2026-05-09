using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public sealed class SingleActiveChildPresenterModule
    {
        private readonly UIPresenter _owner;
        private UIPresenter _activeChild;

        public SingleActiveChildPresenterModule(UIPresenter owner) => _owner = owner;

        public void Initialize(CompositeDisposable disposables)
        {
            UIPresenter.OnUIPresenterAfterShow += OnChildShown;
            UIPresenter.OnUIPresenterAfterHide += OnChildHidden;

            disposables.Add(Disposable.Create(() =>
            {
                UIPresenter.OnUIPresenterAfterShow -= OnChildShown;
                UIPresenter.OnUIPresenterAfterHide -= OnChildHidden;
            }));
        }

        private void OnChildShown(UIPresenter presenter)
        {
            if (presenter?.Parent != _owner)
            {
                return;
            }

            if (_activeChild != null && _activeChild != presenter)
            {
                _activeChild.Hide(CancellationToken.None).Forget();
            }

            _activeChild = presenter;
        }

        private void OnChildHidden(UIPresenter presenter)
        {
            if (_activeChild == presenter)
            {
                _activeChild = null;
            }
        }
    }
}
