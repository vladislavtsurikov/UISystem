using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.ReflectionUtility;
using Random = UnityEngine.Random;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem
{
    [Name("Failure Rate")]
    public class FailureRate : Scatter
    {
        public float Value = 70;

        public override async UniTask Samples(CancellationToken token, BoxArea boxArea, List<Vector2> samples,
            Action<Vector2> onSpawn = null)
        {
            for (var i = samples.Count - 1; i >= 0; i--)
            {
                token.ThrowIfCancellationRequested();

                if (ScatterStack.IsWaitForNextFrame())
                {
                    await UniTask.Yield();
                }

                if (Random.Range(0f, 100f) < Value)
                {
                    samples.RemoveAt(i);
                }
                else
                {
                    onSpawn?.Invoke(samples[i]);
                }
            }
        }
    }
}
