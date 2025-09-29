using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UniRx;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public abstract class UIHandler
    {
        private ChildActivityTracker _childTracker;
        private bool _isActive;
        private bool _isInitialized;
        private SingleActiveUIChildSwitcher _switcher;

        protected ReactiveCollection<UIHandler> Children { get; } = new();
        protected virtual bool AllowMultipleActiveChildren => true;

        public UIHandler Parent { get; private set; }

        public bool IsActive
        {
            get => _isActive;
            private set
            {
                if (value == _isActive)
                {
                    return;
                }

                if (value)
                {
                    BecameActive?.Invoke(this);
                }
            }
        }

        protected internal CompositeDisposable Disposables { get; } = new();

        public event Action<UIHandler> BecameActive;

        public static event Action<UIHandler> OnUIHandlerBeforeShow;
        public static event Action<UIHandler> OnUIHandlerOnShow;
        public static event Action<UIHandler> OnUIHandlerAfterShow;

        public static event Action<UIHandler> OnUIHandlerBeforeHide;
        public static event Action<UIHandler> OnUIHandlerHide;
        public static event Action<UIHandler> OnUIHandlerAfterHide;

        public static event Action<UIHandler> OnUIHandlerDestroyed;

        internal UniTask Initialize(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            if (_isInitialized)
            {
                return UniTask.CompletedTask;
            }

            _isInitialized = true;

            TrackChildActivation();
            InitializeUIHandler(cancellationToken, disposables);
            return UniTask.CompletedTask;
        }

        protected virtual UniTask InitializeUIHandler(CancellationToken cancellationToken,
            CompositeDisposable disposables) => UniTask.CompletedTask;

        protected virtual UniTask BeforeShowUIHandler(CancellationToken ct, CompositeDisposable disposables) =>
            UniTask.CompletedTask;

        protected virtual UniTask OnShowUIHandler(CancellationToken ct, CompositeDisposable disposables) =>
            UniTask.CompletedTask;

        protected virtual UniTask AfterShowUIHandler(CancellationToken ct, CompositeDisposable disposables) =>
            UniTask.CompletedTask;

        protected virtual UniTask BeforeHideUIHandler(CancellationToken ct, CompositeDisposable disposables) =>
            UniTask.CompletedTask;

        protected virtual UniTask OnHideUIHandler(CancellationToken ct, CompositeDisposable disposables) =>
            UniTask.CompletedTask;

        protected virtual UniTask AfterHideUIHandler(CancellationToken ct, CompositeDisposable disposables) =>
            UniTask.CompletedTask;

        protected virtual UniTask
            DestroyUIHandler(bool unload, CancellationToken ct, CompositeDisposable disposables) =>
            UniTask.CompletedTask;

        public virtual void DisposeUIHandler()
        {
        }

        internal void Dispose()
        {
            Disposables.Dispose();
            _childTracker?.Dispose();
            _switcher?.Dispose();

            IsActive = false;
            _isInitialized = false;

            DisposeUIHandler();
        }

        protected async UniTask BeforeShow(CancellationToken ct)
        {
            OnUIHandlerBeforeShow?.Invoke(this);
            await BeforeShowUIHandler(ct, Disposables);
        }

        protected async UniTask OnShow(CancellationToken ct)
        {
            OnUIHandlerOnShow?.Invoke(this);
            await OnShowUIHandler(ct, Disposables);
        }

        protected async UniTask AfterShow(CancellationToken ct)
        {
            OnUIHandlerAfterShow?.Invoke(this);
            await AfterShowUIHandler(ct, Disposables);
        }

        protected async UniTask BeforeHide(CancellationToken ct)
        {
            OnUIHandlerBeforeHide?.Invoke(this);
            await BeforeHideUIHandler(ct, Disposables);
        }

        protected async UniTask OnHide(CancellationToken ct)
        {
            OnUIHandlerHide?.Invoke(this);
            await OnHideUIHandler(ct, Disposables);
        }

        protected async UniTask AfterHide(CancellationToken ct)
        {
            OnUIHandlerAfterHide?.Invoke(this);
            await AfterHideUIHandler(ct, Disposables);
        }

        public async UniTask Show(CancellationToken cancellationToken)
        {
            await UIHandlerUtility.EnsureHandlersReady();

            await BeforeShow(cancellationToken);
            await OnShow(cancellationToken);
            await AfterShow(cancellationToken);

            IsActive = true;

            await InitializeChildren(cancellationToken);
        }

        public async UniTask Hide(CancellationToken cancellationToken)
        {
            await UIHandlerUtility.EnsureHandlersReady();

            await BeforeHide(cancellationToken);
            await OnHide(cancellationToken);
            await AfterHide(cancellationToken);

            IsActive = false;
        }

        protected async UniTask Destroy(bool unload, CancellationToken cancellationToken)
        {
            foreach (UIHandler child in Children)
            {
                await child.Destroy(unload, cancellationToken);
            }

            await DestroyUIHandler(unload, cancellationToken, Disposables);

            Dispose();

            OnUIHandlerDestroyed?.Invoke(this);
        }

        protected internal void AddUIHandlerChild(UIHandler child) => Children.Add(child);

        internal void SetParent(UIHandler parent) => Parent = parent;

        private void TrackChildActivation()
        {
            _childTracker?.Dispose();
            _childTracker = new ChildActivityTracker(Children);

            if (!AllowMultipleActiveChildren)
            {
                _switcher?.Dispose();
                _switcher = new SingleActiveUIChildSwitcher(_childTracker);
            }
        }

        private async Task InitializeChildren(CancellationToken cancellationToken)
        {
            foreach (UIHandler child in Children)
            {
                await child.Initialize(cancellationToken, child.Disposables);
            }
        }
    }
}
