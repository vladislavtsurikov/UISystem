using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.Core.Runtime;
using VladislavTsurikov.Core.Runtime.DependencyInjection;
using VladislavTsurikov.ReflectionUtility.Runtime;
using VladislavTsurikov.UISystem.Runtime.Core.Graph;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public class UIPresenterManager
    {
        private readonly Dictionary<Node, UIPresenter> _activeUIPresenters = new();
        private readonly List<Func<FilterAttribute, bool>> _filters = new();

        internal static UniTask CurrentAddFilterTask { get; private set; }

        protected virtual UIPresenter CreateUIPresenter(Type type)
        {
            return Dependencies.Instantiate(type) as UIPresenter;
        }

        protected virtual void RegisterInContainer(UIPresenter presenter)
        {
            UIPresenterKey key = UIPresenterKey.FromPresenter(presenter);
            Dependencies.BindInstance(presenter.GetType(), key.Id, presenter);
        }

        protected virtual void BeforeRemovePresenter(UIPresenter presenter)
        {
            UIPresenterKey key = UIPresenterKey.FromPresenter(presenter);
            Dependencies.UnbindId(presenter.GetType(), key.Id);
        }

        public async UniTask AddFilter(Func<FilterAttribute, bool> filter,
            CancellationToken cancellationToken = default)
        {
            UniTaskCompletionSource completion = new();
            CurrentAddFilterTask = completion.Task;

            try
            {
                _filters.Add(filter);

                NodeTree tree = GetNodeTree();

                if (tree.Roots.Count == 0)
                {
                    Debug.LogError(
                        "[UIPresenterManager] NodeTree contains no roots. Please check NodeTreeAsset or generation logic.");
                    return;
                }

                foreach (Node root in tree.Roots)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await TraverseMissingUIPresenter(root, null, cancellationToken);
                }
            }
            finally
            {
                completion.TrySetResult();
            }
        }

        public void RemoveFilter(Func<FilterAttribute, bool> filter)
        {
            _filters.Remove(filter);
            CleanupInactivePresenters(_filters);
        }

        public void RemoveExceptGlobalPresenters()
        {
            List<Func<FilterAttribute, bool>> filtersToKeep = _filters
                .Where(f => f(new GlobalFilterAttribute()))
                .ToList();

            CleanupInactivePresenters(filtersToKeep);

            _filters.Clear();
            _filters.AddRange(filtersToKeep);
        }

        private void CleanupInactivePresenters(List<Func<FilterAttribute, bool>> activeFilters)
        {
            List<Node> toRemove = new();

            foreach (KeyValuePair<Node, UIPresenter> pair in _activeUIPresenters)
            {
                Type type = pair.Key.PresenterType;

                if (type == null || !activeFilters.Any(f => type.MatchesAnyFilter(f)))
                {
                    UIPresenter presenter = pair.Value;
                    presenter.Dispose();
                    BeforeRemovePresenter(presenter);
                    toRemove.Add(pair.Key);
                }
            }

            foreach (Node node in toRemove)
            {
                _activeUIPresenters.Remove(node);
            }
        }

        internal async UniTask<TPresenter> CreateDynamicChild<TPresenter>(
            UIPresenter parent,
            string instanceKey,
            bool showAutomatically,
            CancellationToken cancellationToken)
            where TPresenter : UIPresenter
        {
            await UIPresenterResolver.EnsurePresentersReady();

            if (TryGetDynamicChild(parent, instanceKey, out TPresenter existingPresenter))
            {
                if (showAutomatically && !existingPresenter.IsActive.Value)
                {
                    await existingPresenter.Show(cancellationToken);
                }

                return existingPresenter;
            }

            TPresenter presenter = (TPresenter)CreateUIPresenter(typeof(TPresenter));
            AttachPresenter(presenter, parent, instanceKey);
            await presenter.Initialize(cancellationToken, presenter.Disposables);

            if (showAutomatically)
            {
                await presenter.Show(cancellationToken);
            }

            return presenter;
        }

        internal TPresenter GetDynamicChild<TPresenter>(UIPresenter parent, string instanceKey)
            where TPresenter : UIPresenter
        {
            TryGetDynamicChild(parent, instanceKey, out TPresenter presenter);
            return presenter;
        }

        internal bool TryGetDynamicChild<TPresenter>(UIPresenter parent, string instanceKey, out TPresenter presenter)
            where TPresenter : UIPresenter
        {
            if (parent == null || string.IsNullOrEmpty(instanceKey))
            {
                presenter = null;
                return false;
            }

            if (parent.ChildrenModule == null)
            {
                presenter = null;
                return false;
            }

            UIPresenterKey key = UIPresenterKey.FromDynamicParent(parent, instanceKey);
            return UIPresenterResolver.TryResolve(key, out presenter);
        }

        internal async UniTask DestroyDynamicChild<TPresenter>(
            UIPresenter parent,
            string instanceKey,
            bool unload,
            CancellationToken cancellationToken)
            where TPresenter : UIPresenter
        {
            if (!TryGetDynamicChild(parent, instanceKey, out TPresenter presenter))
            {
                return;
            }

            UIPresenterChildrenModule childrenModule = RequireChildrenModule(parent);
            await presenter.Destroy(unload, cancellationToken);
            BeforeRemovePresenter(presenter);
            childrenModule.Remove(presenter);
        }

        private async UniTask TraverseMissingUIPresenter(Node node, UIPresenter parent,
            CancellationToken cancellationToken)
        {
            if (_activeUIPresenters.ContainsKey(node))
            {
                return;
            }

            Type type = node.PresenterType;
            if (type == null)
            {
                Debug.LogError(
                    "[UIPresenterManager] Failed to resolve type for node. Node was probably deserialized incorrectly or type is missing.");
                return;
            }

            if (!_filters.Any(f => type.MatchesAnyFilter(f)))
            {
                return;
            }

            UIPresenter presenter = CreateUIPresenter(type);
            _activeUIPresenters[node] = presenter;
            AttachPresenter(presenter, parent);

            if (presenter.Parent == null)
            {
                await presenter.Initialize(cancellationToken, presenter.Disposables);
            }

            foreach (Node child in node.Children)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await TraverseMissingUIPresenter(child, presenter, cancellationToken);
            }
        }

        private void AttachPresenter(UIPresenter presenter, UIPresenter parent, string instanceKey = null)
        {
            presenter.SetUIPresenterManager(this);

            if (parent != null)
            {
                UIPresenterChildrenModule childrenModule = RequireChildrenModule(parent);
                childrenModule.Add(presenter);
                presenter.SetParent(parent);
            }

            presenter.SetInstanceKey(instanceKey);

            RegisterInContainer(presenter);
        }

        private static UIPresenterChildrenModule RequireChildrenModule(UIPresenter presenter)
        {
            if (presenter?.ChildrenModule != null)
            {
                return presenter.ChildrenModule;
            }

            throw new InvalidOperationException(
                $"[UISystem] Presenter `{presenter?.GetType().FullName}` does not support child presenters.");
        }

        private static NodeTree GetNodeTree()
        {
            NodeTree tree = NodeTreeAsset.Instance?.Tree;
            if (tree != null && tree.Roots.Count > 0)
            {
                return tree;
            }

            return BuildNodeTreeAtRuntime();
        }

        private static NodeTree BuildNodeTreeAtRuntime()
        {
            Type[] allTypes = AllTypesDerivedFrom<UIPresenter>.Types
                .Where(type => !IsDynamicChildType(type))
                .ToArray();
            Dictionary<Type, Node> typeToNode = new();

            foreach (Type type in allTypes)
            {
                Node node = new() {
                    PresenterType = type,
                    Filters = type
                        .GetCustomAttributes(typeof(FilterAttribute), true)
                        .Cast<FilterAttribute>()
                        .Select(filter => filter.GetType())
                        .ToList()
                };

                typeToNode[type] = node;
            }

            List<Node> roots = new();

            foreach (Type type in allTypes)
            {
                Node node = typeToNode[type];
                UIParentAttribute parentAttribute = type
                    .GetCustomAttributes(typeof(UIParentAttribute), true)
                    .Cast<UIParentAttribute>()
                    .FirstOrDefault();

                if (parentAttribute != null && typeToNode.TryGetValue(parentAttribute.ParentType, out Node parentNode))
                {
                    parentNode.Children.Add(node);
                    continue;
                }

                if (parentAttribute != null)
                {
                    Debug.LogWarning(
                        $"[UIPresenterManager] Runtime tree generation skipped `{type.FullName}` because parent `{parentAttribute.ParentType?.FullName}` was not found.");
                    continue;
                }

                roots.Add(node);
            }

            return new NodeTree { Roots = roots };
        }

        private static bool IsDynamicChildType(Type type)
        {
            return Attribute.IsDefined(type, typeof(DynamicUIPresenterChildAttribute), true);
        }
    }
}
