using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.ReflectionUtility;
using Random = UnityEngine.Random;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem
{
    public enum RandomisationType
    {
        None,
        Square,
        Sphere
    }

    [Name("Random Grid")]
    public class RandomGrid : Scatter
    {
        private Vector3 _visualOrigin;

        public float FailureRate = 20;
        public float GridAngle;
        public Vector2 GridStep = new(7, 7);

        public RandomisationType RandomisationType = RandomisationType.Square;
        public bool UniformGrid = true;

        [Range(0, 1)]
        public float Vastness = 1;

        public override async UniTask Samples(CancellationToken token, BoxArea boxArea, List<Vector2> samples,
            Action<Vector2> onSpawn = null)
        {
            UpdateGrid(boxArea.Center, boxArea.BoxSize);

            Vector3 gridOrigin = Vector3.zero;

            Vector3 position = Vector3.zero;
            var halfSpawnRange = boxArea.Radius;

            Vector3 maxPosition = gridOrigin + Vector3.one * (halfSpawnRange * 2);

            for (position.x = gridOrigin.x; position.x < maxPosition.x; position.x += GridStep.x)
            for (position.z = gridOrigin.z; position.z < maxPosition.y; position.z += GridStep.y)
            {
                token.ThrowIfCancellationRequested();

                if (ScatterStack.IsWaitForNextFrame())
                {
                    await UniTask.Yield();
                }

                Vector3 newLocalPosition = Vector3Ex.RotatePointAroundPivot(position,
                    new Vector3(maxPosition.x / 2, 0, maxPosition.z / 2), Quaternion.AngleAxis(GridAngle, Vector3.up));
                var offsetLocalPosition = new Vector2(_visualOrigin.x + newLocalPosition.x,
                    _visualOrigin.z + newLocalPosition.z);
                offsetLocalPosition = GetCurrentRandomPosition(offsetLocalPosition);

                if (Random.Range(0f, 100f) > FailureRate)
                {
                    if (boxArea.Contains(offsetLocalPosition))
                    {
                        onSpawn?.Invoke(offsetLocalPosition);
                        samples.Add(offsetLocalPosition);
                    }
                }
            }
        }

        private void UpdateGrid(Vector3 dragPoint, float size)
        {
            Vector3 gridOrigin = Vector3.zero;
            var localGridStep = new Vector3(GridStep.x, GridStep.y, 1);
            Vector3 gridNormal = Vector3.up;

            var halfSpawnRange = size / 2;

            var point = new Vector3(dragPoint.x - halfSpawnRange, 0, dragPoint.z - halfSpawnRange);

            Matrix4x4 gridMatrix = Matrix4x4.TRS(Vector3.zero,
                                       Quaternion.AngleAxis(GridAngle, gridNormal) *
                                       Quaternion.LookRotation(gridNormal),
                                       Vector3.one)
                                   * Matrix4x4.TRS(gridOrigin, Quaternion.identity, localGridStep);

            Vector3 gridSpacePoint = gridMatrix.inverse.MultiplyPoint(point);

            gridSpacePoint = new Vector3(Mathf.Round(gridSpacePoint.x), Mathf.Round(gridSpacePoint.y),
                gridSpacePoint.z);

            Vector3 snappedHitPoint = gridMatrix.MultiplyPoint(gridSpacePoint);
            _visualOrigin = snappedHitPoint;
        }

        private Vector2 GetRandomSquarePoint(Vector2 sample)
        {
            var halfDistanceX = GridStep.x / 2;
            var halfDistanceY = GridStep.y / 2;
            var distanceOffset = new Vector2(Random.Range(-halfDistanceX, halfDistanceX),
                Random.Range(-halfDistanceY, halfDistanceY));
            return sample + distanceOffset;
        }

        private Vector2 GetRandomSpherePoint(Vector2 sample)
        {
            var halfDistance = Mathf.Lerp(0, GridStep.x / 2, Vastness);
            var distanceOffset = new Vector2(Random.Range(-halfDistance, halfDistance),
                Random.Range(-halfDistance, halfDistance));
            return sample + distanceOffset;
        }

        private Vector2 GetCurrentRandomPosition(Vector2 sample)
        {
            switch (RandomisationType)
            {
                case RandomisationType.Square:
                {
                    return GetRandomSquarePoint(sample);
                }
                case RandomisationType.Sphere:
                {
                    return GetRandomSpherePoint(sample);
                }
            }

            return sample;
        }
    }
}
