using System;
using OdinSerializer;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using VladislavTsurikov.AutoUnmanagedPropertiesDispose.Runtime;
using VladislavTsurikov.UnityUtility.Runtime;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.Common.TerrainSystem
{
    [Serializable]
    [TerrainHelper(typeof(Terrain), "Unity terrain")]
    public class UnityTerrain : TerrainHelper
    {
        private readonly NativeArrayProperty<float> _heights = new();

        [OdinSerialize]
        private Terrain _terrain;

        public override void Init() => _terrain = (Terrain)Target;

        public override Bounds GetTerrainBounds()
        {
            TerrainData terrainData = _terrain.terrainData;
            return new Bounds(terrainData.bounds.center + _terrain.transform.position, terrainData.bounds.size);
        }

        public override JobHandle SetCellHeight(NativeArray<Bounds> cellBoundsList, float minHeightCells,
            Rect cellBoundsRect, JobHandle dependsOn = default)
        {
            SetDataFromTerrain();

            Rect terrainRect = RectExtension.CreateRectFromBounds(GetTerrainBounds());

            if (cellBoundsRect.Overlaps(terrainRect))
            {
                var unityTerranCellSampleJob = new UnityTerranCellSampleJob
                {
                    InputHeights = _heights.NativeArray,
                    CellBoundsList = cellBoundsList,
                    HeightMapScale = _terrain.terrainData.heightmapScale,
                    HeightmapHeight = _terrain.terrainData.heightmapResolution,
                    HeightmapWidth = _terrain.terrainData.heightmapResolution,
                    TerrainPosition = _terrain.transform.position,
                    MinHeightCells = minHeightCells,
                    TerrainRect = terrainRect
                };

                JobHandle handle = unityTerranCellSampleJob.Schedule(cellBoundsList.Length, 32, dependsOn);
                return handle;
            }

            return dependsOn;
        }

        public override void RefreshData() => SetDataFromTerrain();

        public override void OnDisable() => _heights?.DisposeUnmanagedMemory();

        private void SetDataFromTerrain()
        {
            TerrainData terrainData = _terrain.terrainData;

            var hs = _terrain.terrainData.GetHeights(0, 0, terrainData.heightmapResolution,
                terrainData.heightmapResolution);

            _heights.ChangeNativeArray(new NativeArray<float>(
                terrainData.heightmapResolution * terrainData.heightmapResolution, Allocator.Persistent));
            _heights.NativeArray.CopyFromFast(hs);
        }

        ~UnityTerrain() => _heights.DisposeUnmanagedMemory();
    }
}
