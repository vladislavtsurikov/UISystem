using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public abstract class UIPresenter
    {
        internal readonly BoolReactiveProperty IsActive = new();
        private bool _isInitialized;

        public static event Action<UIPresenter> OnUIPresenterBeforeShow;
        public static event Action<UIPresenter> OnUIPresenterOnShow;
        public static event Action<UIPresenter> OnUIPresenterAfterShow;

        public static event Action<UIPresenter> OnUIPresenterBeforeHide;
        public static event Action<UIPresenter> OnUIPresenterHide;
        public static event Action<UIPresenter> OnUIPresenterAfterHide;

        public static event Action<UIPresenter> OnUIPresenterDestroyed;

        public UIPresenter Parent { get; private set; }
        public string InstanceKey { get; private set; }
        internal UIPresenterManager UIPresenterManager { get; private set; }
        public UIPresenterChildrenModule ChildrenModule { get; }

        public ViewResolver ViewResolver { get; }

        protected virtual bool UsesParentBindingContext => false;

        protected internal CompositeDisposable Disposables { get; } = new();

        protected UIPresenter()
            : this(true)
        {
        }

        protected UIPresenter(bool supportsChildren)
        {
            ViewResolver = new ViewResolver(this);

            if (supportsChildren)
            {
                ChildrenModule = new UIPresenterChildrenModule(this);
            }
        }

        internal UniTask Initialize(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            if (_isInitialized)
            {
                return UniTask.CompletedTask;
            }

            _isInitialized = true;
            InitializeUIPresenter(cancellationToken, disposables);
            return UniTask.CompletedTask;
        }

        protected virtual UniTask InitializeUIPresenter(CancellationToken cancellationToken,
            CompositeDisposable disposables)
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask BeforeShowUIPresenter(CancellationToken ct, CompositeDisposable disposables)
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask OnShowUIPresenter(CancellationToken ct, CompositeDisposable disposables)
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask AfterShowUIPresenter(CancellationToken ct, CompositeDisposable disposables)
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask BeforeHideUIPresenter(CancellationToken ct, CompositeDisposable disposables)
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask OnHideUIPresenter(CancellationToken ct, CompositeDisposable disposables)
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask AfterHideUIPresenter(CancellationToken ct, CompositeDisposable disposables)
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask DestroyUIPresenter(bool unload, CancellationToken ct, CompositeDisposable disposables)
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask EnsurePresenterRoot(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        protected virtual void ShowPresenterRoot()
        {
        }

        protected virtual void HidePresenterRoot()
        {
        }

        protected virtual UniTask DestroyPresenterRoot(bool unload, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public virtual void DisposeUIPresenter()
        {
        }

        internal void Dispose()
        {
            IsActive.Value = false;
            Disposables.Dispose();
            ChildrenModule?.Dispose();

            _isInitialized = false;
            IsActive.Dispose();

            DisposeUIPresenter();
        }

        protected async UniTask BeforeShow(CancellationToken ct)
        {
            OnUIPresenterBeforeShow?.Invoke(this);
            await EnsurePresenterRoot(ct);
            await BeforeShowUIPresenter(ct, Disposables);
        }

        protected async UniTask OnShow(CancellationToken ct)
        {
            OnUIPresenterOnShow?.Invoke(this);
            ShowPresenterRoot();
            await OnShowUIPresenter(ct, Disposables);
        }

        protected async UniTask AfterShow(CancellationToken ct)
        {
            OnUIPresenterAfterShow?.Invoke(this);
            await AfterShowUIPresenter(ct, Disposables);
        }

        protected async UniTask BeforeHide(CancellationToken ct)
        {
            OnUIPresenterBeforeHide?.Invoke(this);
            await BeforeHideUIPresenter(ct, Disposables);
        }

        protected async UniTask OnHide(CancellationToken ct)
        {
            OnUIPresenterHide?.Invoke(this);
            HidePresenterRoot();
            await OnHideUIPresenter(ct, Disposables);
        }

        protected async UniTask AfterHide(CancellationToken ct)
        {
            OnUIPresenterAfterHide?.Invoke(this);
            await AfterHideUIPresenter(ct, Disposables);
        }

        public async UniTask Show(CancellationToken cancellationToken)
        {
            await UIPresenterResolver.EnsurePresentersReady();

            await BeforeShow(cancellationToken);
            await OnShow(cancellationToken);
            await AfterShow(cancellationToken);

            IsActive.Value = true;

            if (ChildrenModule != null)
            {
                await ChildrenModule.InitializeChildren(cancellationToken);
                await ChildrenModule.ShowDynamicChildren(cancellationToken);
            }
        }

        public async UniTask Hide(CancellationToken cancellationToken)
        {
            await UIPresenterResolver.EnsurePresentersReady();

            await BeforeHide(cancellationToken);
            await OnHide(cancellationToken);
            await AfterHide(cancellationToken);
            if (ChildrenModule != null)
            {
                await ChildrenModule.HideChildren(cancellationToken);
            }

            IsActive.Value = false;
        }

        internal async UniTask Destroy(bool unload, CancellationToken cancellationToken)
        {
            if (ChildrenModule != null)
            {
                await ChildrenModule.DestroyAll(unload, cancellationToken);
            }

            await DestroyPresenterRoot(unload, cancellationToken);
            await DestroyUIPresenter(unload, cancellationToken, Disposables);

            Dispose();

            OnUIPresenterDestroyed?.Invoke(this);
        }

        internal virtual (Type presenterType, string instanceKey) ResolveBindingContext()
        {
            return !UsesParentBindingContext
                ? (GetType(), InstanceKey)
                : (Parent?.GetType() ?? GetType(), Parent?.InstanceKey);
        }

        protected TView GetView<TView>(string bindingId, int index = 0)
        {
            return ViewResolver.GetView<TView>(bindingId, index);
        }

        protected UniTask<TPresenter> CreateDynamicChild<TPresenter>(
            string instanceKey,
            bool showAutomatically = false,
            CancellationToken cancellationToken = default)
            where TPresenter : UIPresenter
        {
            return RequireChildrenModule().CreateDynamicChild<TPresenter>(instanceKey, showAutomatically,
                cancellationToken);
        }

        protected TPresenter GetDynamicChild<TPresenter>(string instanceKey)
            where TPresenter : UIPresenter
        {
            return RequireChildrenModule().GetDynamicChild<TPresenter>(instanceKey);
        }

        protected bool TryGetDynamicChild<TPresenter>(string instanceKey, out TPresenter presenter)
            where TPresenter : UIPresenter
        {
            return RequireChildrenModule().TryGetDynamicChild(instanceKey, out presenter);
        }

        protected UniTask DestroyDynamicChild<TPresenter>(
            string instanceKey,
            bool unload,
            CancellationToken cancellationToken = default)
            where TPresenter : UIPresenter
        {
            return RequireChildrenModule().DestroyChild<TPresenter>(instanceKey, unload, cancellationToken);
        }

        private UIPresenterChildrenModule RequireChildrenModule()
        {
            if (ChildrenModule != null)
            {
                return ChildrenModule;
            }

            throw new InvalidOperationException(
                $"[UISystem] Presenter `{GetType().FullName}` does not support child presenters.");
        }

        internal void SetParent(UIPresenter parent)
        {
            Parent = parent;
        }

        internal void SetInstanceKey(string instanceKey)
        {
            InstanceKey = instanceKey;
        }

        internal void SetUIPresenterManager(UIPresenterManager uiHandlerManager)
        {
            UIPresenterManager = uiHandlerManager;
        }
    }
}
