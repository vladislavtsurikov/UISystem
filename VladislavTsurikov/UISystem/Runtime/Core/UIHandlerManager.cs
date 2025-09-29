using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.Core.Graph;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public abstract class UIHandlerManager
    {
        private readonly Dictionary<Node, UIHandler> _activeUIHandlers = new();
        private readonly List<Func<FilterAttribute, bool>> _filters = new();

        internal static UniTask CurrentAddFilterTask { get; private set; }

        protected virtual UIHandler CreateUIHandler(Type type) => (UIHandler)Activator.CreateInstance(type);

        protected virtual void RegisterInContainer(UIHandler handler)
        {
        }

        protected virtual void BeforeRemoveHandler(UIHandler handler)
        {
        }

        public async UniTask AddFilter(Func<FilterAttribute, bool> filter,
            CancellationToken cancellationToken = default)
        {
            var completion = new UniTaskCompletionSource();
            CurrentAddFilterTask = completion.Task;

            try
            {
                _filters.Add(filter);

                NodeTree tree = NodeTreeAsset.Instance.Tree;

                if (tree.Roots.Count == 0)
                {
                    Debug.LogError(
                        "[UIHandlerManager] NodeTree contains no roots. Please check NodeTreeAsset or generation logic.");
                    return;
                }

                foreach (Node root in tree.Roots)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await TraverseMissingUIHandler(root, null, cancellationToken);
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
            CleanupInactiveHandlers(_filters);
        }

        public void RemoveExceptGlobalHandlers()
        {
            var filtersToKeep = _filters
                .Where(f => f(new GlobalFilterAttribute()))
                .ToList();

            CleanupInactiveHandlers(filtersToKeep);

            _filters.Clear();
            _filters.AddRange(filtersToKeep);
        }

        private void CleanupInactiveHandlers(List<Func<FilterAttribute, bool>> activeFilters)
        {
            var toRemove = new List<Node>();

            foreach (KeyValuePair<Node, UIHandler> pair in _activeUIHandlers)
            {
                Type type = pair.Key.HandlerType;

                if (type == null || !activeFilters.Any(f => type.MatchesAnyFilter(f)))
                {
                    UIHandler handler = pair.Value;
                    handler.Dispose();
                    BeforeRemoveHandler(handler);
                    toRemove.Add(pair.Key);
                }
            }

            foreach (Node node in toRemove)
            {
                _activeUIHandlers.Remove(node);
            }
        }

        private async UniTask TraverseMissingUIHandler(Node node, UIHandler parent, CancellationToken cancellationToken)
        {
            if (_activeUIHandlers.ContainsKey(node))
            {
                return;
            }

            Type type = node.HandlerType;
            if (type == null)
            {
                Debug.LogError(
                    "[UIHandlerManager] Failed to resolve type for node. Node was probably deserialized incorrectly or type is missing.");
                return;
            }

            if (!_filters.Any(f => type.MatchesAnyFilter(f)))
            {
                return;
            }

            UIHandler handler = CreateUIHandler(type);
            _activeUIHandlers[node] = handler;

            if (parent != null)
            {
                parent.AddUIHandlerChild(handler);
                handler.SetParent(parent);
            }

            RegisterInContainer(handler);

            if (handler.Parent == null)
            {
                await handler.Initialize(cancellationToken, handler.Disposables);
            }

            foreach (Node child in node.Children)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await TraverseMissingUIHandler(child, handler, cancellationToken);
            }
        }
    }
}
