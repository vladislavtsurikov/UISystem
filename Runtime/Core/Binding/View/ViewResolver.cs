using System;
using VladislavTsurikov.Core.Runtime.DependencyInjection;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public sealed class ViewResolver
    {
        private readonly UIPresenter _presenter;

        public ViewResolver(UIPresenter presenter) => _presenter = presenter;

        public TView GetView<TView>(string bindingId, int index = 0)
        {
            ViewKey key = CreateKey<TView>(bindingId, index);
            return ResolveWithKey<TView>(key);
        }

        private TView ResolveWithKey<TView>(ViewKey key)
        {
            if (Dependencies.TryResolveId(typeof(TView), key.Id, out object instance) &&
                instance is TView typedView)
            {
                return typedView;
            }

            throw new InvalidOperationException(
                $"[UISystem] Failed to resolve bound view `{typeof(TView).Name}` with id `{key.Id}`.");
        }

        private ViewKey CreateKey<TView>(string bindingId, int index)
        {
            (Type presenterType, string instanceKey) = _presenter.ResolveBindingContext();
            return new ViewKey(typeof(TView), presenterType, bindingId, index, instanceKey);
        }
    }
}
