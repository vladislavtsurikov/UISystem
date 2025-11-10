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
    [Name("Random Point")]
    public class RandomPoint : Scatter
    {
        public int MaxChecks = 15;
        public int MinChecks = 15;

        public override async UniTask Samples(CancellationToken token, BoxArea boxArea, List<Vector2> samples,
            Action<Vector2> onSpawn = null)
        {
            var numberOfChecks = Random.Range(MinChecks, MaxChecks);

            for (var checks = 0; checks < numberOfChecks; checks++)
            {
                token.ThrowIfCancellationRequested();

                if (ScatterStack.IsWaitForNextFrame())
                {
                    await UniTask.Yield();
                }

                Vector2 point = GetRandomPoint(boxArea);
                onSpawn?.Invoke(point);
                samples.Add(point);
            }
        }

        private Vector2 GetRandomPoint(BoxArea boxArea)
        {
            Vector2 spawnOffset = new Vector3(Random.Range(-boxArea.Radius, boxArea.Radius),
                Random.Range(-boxArea.Radius, boxArea.Radius));
            return new Vector2(spawnOffset.x + boxArea.RayHit.Point.x, spawnOffset.y + boxArea.RayHit.Point.z);
        }
    }
}
