using System;
using OdinSerializer;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace VladislavTsurikov.RendererStack.Runtime.Common.TerrainSystem
{
    [Serializable]
    public abstract class TerrainHelper
    {
        [OdinSerialize]
        public Behaviour Target;

        public void InternalInit(Behaviour terrain)
        {
            Target = terrain;
            Init();
        }

        public abstract void Init();
        public abstract Bounds GetTerrainBounds();

        public abstract JobHandle SetCellHeight(NativeArray<Bounds> cellBoundsList, float minHeightCells,
            Rect cellBoundsRect, JobHandle dependsOn = default);

        public abstract void RefreshData();
        public abstract void OnDisable();
    }
}
