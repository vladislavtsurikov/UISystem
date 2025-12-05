using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem
{
    [Name("Tiles")]
    public class Tiles : Scatter
    {
        public Vector2 Size = new(1, 1);

        public override async UniTask Samples(CancellationToken token, BoxArea boxArea, List<Vector2> samples,
            Action<Vector2> onSpawn = null)
        {
            var tileMapSize = new Vector2Int(
                Mathf.RoundToInt(boxArea.BoxSize),
                Mathf.RoundToInt(boxArea.BoxSize)
            );

            for (var x = 0; x < tileMapSize.x; x++)
            for (var y = 0; y < tileMapSize.y; y++)
            {
                token.ThrowIfCancellationRequested();

                if (ScatterStack.IsWaitForNextFrame())
                {
                    await UniTask.Yield();
                }

                var cellCenter = new Vector2(
                    x * Size.x + Size.x / 2,
                    y * Size.y + Size.y / 2
                );

                if (boxArea.Contains(cellCenter))
                {
                    onSpawn?.Invoke(cellCenter);
                    samples.Add(cellCenter);
                }
            }
        }
    }
}
