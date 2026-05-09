using System;
using System.Collections.Generic;
using VladislavTsurikov.Core.Runtime.DependencyInjection;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public abstract class ViewBindingScope : IDisposable
    {
        private readonly List<ViewKey> _records = new();

        protected UIPresenter UIPresenter { get; }

        protected ViewBindingScope(UIPresenter presenter) => UIPresenter = presenter;

        public void Dispose()
        {
            foreach (ViewKey key in _records)
            {
                Dependencies.UnbindId(key.ViewType, key.Id);
            }

            _records.Clear();
        }

        protected void RegisterBindings<TNode>(
            IEnumerable<TNode> nodes,
            Func<TNode, string> getBindingId,
            Func<TNode, string> getInstanceKey = null)
            where TNode : class
        {
            Dictionary<(Type, string), int> repeats = new();

            foreach (TNode node in nodes)
            {
                if (node == null)
                {
                    continue;
                }

                string rawBindingId = getBindingId(node);
                if (string.IsNullOrEmpty(rawBindingId))
                {
                    continue;
                }

                Type type = node.GetType();
                (Type, string) repeatKey = (type, rawBindingId);
                repeats.TryGetValue(repeatKey, out int index);
                repeats[repeatKey] = index + 1;

                ViewKey key = new(
                    type,
                    UIPresenter.GetType(),
                    rawBindingId,
                    index,
                    getInstanceKey?.Invoke(node));

                Dependencies.BindInstance(type, key.Id, node);

                _records.Add(key);
            }
        }
    }
}
