using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem
{
    public class ScatterStack : ComponentStackOnlyDifferentTypes<Scatter>
    {
        private WaitingNextFrame _waitingNextFrame;

        public async UniTask Samples(BoxArea boxArea, Action<Vector2> onAddSample, CancellationToken token = default)
        {
            var enabledScatter = new List<Scatter>(_elementList.FindAll(scatter => scatter.Active));

            var samples = new List<Vector2>();

            for (var i = 0; i < enabledScatter.Count; i++)
            {
                await enabledScatter[i]
                    .Samples(token, boxArea, samples, i == enabledScatter.Count - 1 ? onAddSample : null);
            }
        }

        public void SetWaitingNextFrame(WaitingNextFrame waitingNextFrame) => _waitingNextFrame = waitingNextFrame;

        public bool IsWaitForNextFrame()
        {
            if (_waitingNextFrame == null)
            {
                return false;
            }

            return _waitingNextFrame.IsWaitForNextFrame();
        }
    }
}
