using System;
using System.Threading;
using Assemblies.VladislavTsurikov.ComponentStack.Runtime.SingleElementStack;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.ActionFlow.Runtime.Events.GameObjectLifecycle;

namespace VladislavTsurikov.ActionFlow.Runtime.Events
{
    [ExecuteInEditMode]
    public class Trigger : SerializedMonoBehaviour
    {
        private CancellationTokenSource _cancellationTokenSource;

        private Action<int> _onCollectionChangedHandler;

        [OdinSerialize]
        public ActionCollection ActionCollection = new();

        [OdinSerialize]
        public SingleElementStack<Event> SingleElementStack = new();

        private void OnEnable()
        {
            InitializeSingleElementStack();
            InitializeCancellation();

            _onCollectionChangedHandler = _ =>
            {
                EventCallbacksUtility.SetupEventCallbacksForce(gameObject, SingleElementStack.GetElement());
            };
            SingleElementStack.ElementAdded += _onCollectionChangedHandler;

            EventCallbacksUtility.SetupEventCallbacksForce(gameObject, SingleElementStack.GetElement());
        }

        private void OnDisable()
        {
            _cancellationTokenSource?.Cancel();
            SingleElementStack.OnDisable();
            UnsubscribeFromCollectionChanges();
        }

        internal void Run() => ActionCollection.Run(_cancellationTokenSource.Token).Forget();

        private void InitializeSingleElementStack()
        {
            SingleElementStack ??= new SingleElementStack<Event>();
            SingleElementStack.Setup(true, new object[] { this });
            SingleElementStack.CreateFirstElementIfNecessary(typeof(OnEnableEvent));
        }

        private void InitializeCancellation() => _cancellationTokenSource = new CancellationTokenSource();

        private void UnsubscribeFromCollectionChanges()
        {
            if (_onCollectionChangedHandler != null)
            {
                SingleElementStack.ElementAdded -= _onCollectionChangedHandler;
            }
        }
    }
}
