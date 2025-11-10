using System;
using UniRx;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public class ChildActivityTracker : IDisposable
    {
        private readonly ReactiveCollection<UIHandler> _children;
        private readonly CompositeDisposable _childrenChanges = new();

        public ChildActivityTracker(ReactiveCollection<UIHandler> children)
        {
            _children = children;

            SubscribeToChildren();
            TrackActivations();
        }

        public void Dispose() => _childrenChanges.Dispose();

        public event Action<UIHandler> BecameActive;

        private void SubscribeToChildren()
        {
            _childrenChanges.Clear();

            _children
                .ObserveReset()
                .Subscribe(_ => TrackActivations())
                .AddTo(_childrenChanges);

            _children
                .ObserveAdd()
                .Subscribe(_ => TrackActivations())
                .AddTo(_childrenChanges);
        }

        private void TrackActivations()
        {
            foreach (UIHandler child in _children)
            {
                SubscribeTo(child);
            }
        }

        private void SubscribeTo(UIHandler child)
        {
            child.BecameActive -= OnActive;
            child.BecameActive += OnActive;
            return;

            void OnActive(UIHandler activated)
            {
                BecameActive?.Invoke(activated);
            }
        }
    }
}
