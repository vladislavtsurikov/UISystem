using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace VladislavTsurikov.Utility.Runtime
{
    public class AsyncTaskQueue
    {
        private readonly Queue<Func<UniTask>> _queue = new();
        private bool _isRunning;

        public void Enqueue(Func<UniTask> task)
        {
            _queue.Enqueue(task);

            if (!_isRunning)
            {
                RunQueue().Forget();
            }
        }

        private async UniTaskVoid RunQueue()
        {
            _isRunning = true;

            while (_queue.Count > 0)
            {
                var task = _queue.Dequeue();

                try
                {
                    await task();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            _isRunning = false;
        }

        public void Clear()
        {
            _queue.Clear();
        }

        public bool IsEmpty => _queue.Count == 0;
        public bool IsRunning => _isRunning;
    }
}