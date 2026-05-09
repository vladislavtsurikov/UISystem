using System.Threading;
using Cysharp.Threading.Tasks;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public abstract class UISpawnOperation<TSelf, TParent, TInstance, TLoader, TBinder>
        where TSelf : UISpawnOperation<TSelf, TParent, TInstance, TLoader, TBinder>, new()
        where TInstance : class
    {
        private string _name;
        private TParent _parent;
        private bool _visible;

        public static TSelf Spawn()
        {
            return new TSelf();
        }

        public TSelf WithParent(TParent parent)
        {
            _parent = parent;
            return (TSelf)this;
        }

        public TSelf WithName(string name)
        {
            _name = name;
            return (TSelf)this;
        }

        public async UniTask<TInstance> Execute(TLoader loader, TBinder binder, CancellationToken cancellationToken)
        {
            TInstance instance = await CreateInstance(loader, _parent, cancellationToken);
            if (instance == null)
            {
                return null;
            }

            ApplyName(instance, _name);
            ApplyVisibility(instance, _visible);
            AttachToParent(instance, _parent);
            Bind(instance, binder);

            return instance;
        }

        protected TSelf SetVisibleState(bool visible)
        {
            _visible = visible;
            return (TSelf)this;
        }

        protected abstract UniTask<TInstance> CreateInstance(
            TLoader loader,
            TParent parent,
            CancellationToken cancellationToken);

        protected abstract void ApplyName(TInstance instance, string name);

        protected abstract void ApplyVisibility(TInstance instance, bool visible);

        protected abstract void AttachToParent(TInstance instance, TParent parent);

        protected abstract void Bind(TInstance instance, TBinder binder);
    }
}
