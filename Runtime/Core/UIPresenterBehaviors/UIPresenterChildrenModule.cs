using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UniRx;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public sealed class UIPresenterChildrenModule : IDisposable
    {
        private readonly UIPresenter _owner;

        public ReactiveCollection<UIPresenter> All { get; } = new();

        public UIPresenterChildrenModule(UIPresenter owner) => _owner = owner;

        public void Dispose()
        {
            All.Clear();
        }

        public void Add(UIPresenter child)
        {
            All.Add(child);
        }

        public void Remove(UIPresenter child)
        {
            All.Remove(child);
        }

        public UniTask<TPresenter> CreateDynamicChild<TPresenter>(
            string instanceKey,
            bool showAutomatically = false,
            CancellationToken cancellationToken = default)
            where TPresenter : UIPresenter
        {
            return _owner.UIPresenterManager.CreateDynamicChild<TPresenter>(_owner, instanceKey, showAutomatically,
                cancellationToken);
        }

        public TPresenter GetDynamicChild<TPresenter>(string instanceKey)
            where TPresenter : UIPresenter
        {
            return _owner.UIPresenterManager.GetDynamicChild<TPresenter>(_owner, instanceKey);
        }

        public bool TryGetDynamicChild<TPresenter>(string instanceKey, out TPresenter presenter)
            where TPresenter : UIPresenter
        {
            return _owner.UIPresenterManager.TryGetDynamicChild(_owner, instanceKey, out presenter);
        }

        public UniTask DestroyChild<TPresenter>(
            string instanceKey,
            bool unload,
            CancellationToken cancellationToken = default)
            where TPresenter : UIPresenter
        {
            return _owner.UIPresenterManager.DestroyDynamicChild<TPresenter>(_owner, instanceKey, unload,
                cancellationToken);
        }

        public async UniTask DestroyAll(bool unload, CancellationToken cancellationToken)
        {
            foreach (UIPresenter child in All)
            {
                await child.Destroy(unload, cancellationToken);
            }
        }

        public async Task InitializeChildren(CancellationToken cancellationToken)
        {
            foreach (UIPresenter child in All)
            {
                if (IsDynamicChild(child))
                {
                    continue;
                }

                await child.Initialize(cancellationToken, child.Disposables);
            }
        }

        public async Task HideChildren(CancellationToken cancellationToken)
        {
            List<UIPresenter> childrenToHide = new();

            foreach (UIPresenter child in All)
            {
                if (child.IsActive.Value)
                {
                    childrenToHide.Add(child);
                }
            }

            foreach (UIPresenter child in childrenToHide)
            {
                await child.Hide(cancellationToken);
            }
        }

        public async Task ShowDynamicChildren(CancellationToken cancellationToken)
        {
            foreach (UIPresenter child in All)
            {
                if (IsDynamicChild(child))
                {
                    await child.Show(cancellationToken);
                }
            }
        }

        private static bool IsDynamicChild(UIPresenter child)
        {
            return child != null &&
                   Attribute.IsDefined(child.GetType(), typeof(DynamicUIPresenterChildAttribute), true);
        }
    }
}
