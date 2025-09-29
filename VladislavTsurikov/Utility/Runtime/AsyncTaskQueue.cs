using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace VladislavTsurikov.Utility.Runtime
{
    public class AsyncTaskQueue
    {
        private readonly Queue<Func<UniTask>> _queue = new();

        public bool IsEmpty => _queue.Count == 0;
        public bool IsRunning { get; private set; }

        public void Enqueue(Func<UniTask> task)
        {
            _queue.Enqueue(task);

            if (!IsRunning)
            {
                RunQueue().Forget();
            }
        }

        private async UniTaskVoid RunQueue()
        {
            IsRunning = true;

            while (_queue.Count > 0)
            {
                Func<UniTask> task = _queue.Dequeue();

                try
                {
                    await task();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            IsRunning = false;
        }

        public void Clear() => _queue.Clear();
    }
}
