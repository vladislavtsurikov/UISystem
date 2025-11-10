#if GRIFFIN_2020 || GRIFFIN_2021
using System;
using Pinwheel.Griffin;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using VladislavTsurikov.AutoUnmanagedPropertiesDispose.Scripts.UnmanagedProperties;
using VladislavTsurikov.Extensions.Scripts;
using VladislavTsurikov.InstantRenderer.CommonScripts.Scripts.TerrainSystem.Jobs;
using VladislavTsurikov.OdinSerializer.Core.Misc;

namespace VladislavTsurikov.InstantRenderer.CommonScripts.Scripts.TerrainSystem.Terrains
{
    /*[Serializable]
    [TerrainHelper(typeof(GStylizedTerrain), "Polaris terrain")]
    public class PolarisTerrain : TerrainHelper
    {
        [OdinSerialize] 
        private GStylizedTerrain _terrain;
        private NativeArrayProperty<float> _heights = new NativeArrayProperty<float>();

        public override void Init()
        {
            _terrain = (GStylizedTerrain)Target;
        }

        public override Bounds GetTerrainBounds()
        {
            return _terrain.Bounds;
        }

        public override JobHandle SetCellHeight(NativeArray<Bounds> cellBoundsList, float minHeightCells,
            Rect cellBoundsRect, JobHandle dependsOn = default(JobHandle))
        {
            SetDataFromTerrain();

            Vector2 _heightmapTexelSize = _terrain.TerrainData.Geometry.HeightMap.texelSize;
            Vector3 _heightmapScale = new Vector3(_heightmapTexelSize.x * _terrain.TerrainData.Geometry.Width, _terrain.TerrainData.Geometry.Height, _heightmapTexelSize.y * _terrain.TerrainData.Geometry.Length);

            Rect terrainRect = RectExtension.CreateRectFromBounds(GetTerrainBounds());

            if (cellBoundsRect.Overlaps(terrainRect))
            {
                UnityTerranCellSampleJob unityTerranCellSampleJob = new UnityTerranCellSampleJob
                {
                    InputHeights = _heights.NativeArray,
                    CellBoundsList = cellBoundsList,
                    HeightMapScale = _heightmapScale,
                    HeightmapHeight = _terrain.TerrainData.Geometry.HeightMapResolution,
                    HeightmapWidth = _terrain.TerrainData.Geometry.HeightMapResolution,
                    TerrainPosition = _terrain.transform.position,
                    MinHeightCells = minHeightCells,
                    TerrainRect = terrainRect
                };

                JobHandle handle = unityTerranCellSampleJob.Schedule(cellBoundsList.Length, 32, dependsOn);
                return handle;
            }

            return dependsOn;
        }

        public override void RefreshData()
        {
            SetDataFromTerrain();
        }

        public override void OnDisable()
        {
            _heights.DisposeUnmanagedMemory();
        }

        void SetDataFromTerrain()
        {            
            float[,] hs = _terrain.TerrainData.Geometry.GetHeights();

            _heights.ChangeNativeArray(new NativeArray<float>(_terrain.TerrainData.Geometry.HeightMapResolution * _terrain.TerrainData.Geometry.HeightMapResolution, Allocator.Persistent));
            _heights.NativeArray.CopyFromFast(hs);
        }

        ~PolarisTerrain()
        {
            _heights.DisposeUnmanagedMemory();
        }
    }*/
}
#endif
