using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public class SingleActiveUIChildSwitcher : IDisposable
    {
        private readonly ChildActivityTracker _tracker;
        private UIHandler _current;

        public SingleActiveUIChildSwitcher(ChildActivityTracker tracker)
        {
            _tracker = tracker;
            _tracker.BecameActive += OnAnyChildActivated;
        }

        public void Dispose() => _tracker.BecameActive -= OnAnyChildActivated;

        /// <summary>
        ///     Invoked when active UIHandler changes: (previous, current).
        /// </summary>
        public event Action<UIHandler, UIHandler> OnActiveSwitched;

        private void OnAnyChildActivated(UIHandler next)
        {
            if (_current != null && _current != next)
            {
                OnActiveSwitched?.Invoke(_current, next);
                _current.Hide(CancellationToken.None).Forget();
            }

            _current = next;
        }
    }
}
