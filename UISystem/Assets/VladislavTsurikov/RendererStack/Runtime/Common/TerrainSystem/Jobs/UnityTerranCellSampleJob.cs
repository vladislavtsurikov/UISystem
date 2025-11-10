using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.Common.TerrainSystem
{
    [BurstCompile(CompileSynchronously = true)]
    public struct UnityTerranCellSampleJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<float> InputHeights;

        public NativeArray<Bounds> CellBoundsList;
        public int HeightmapWidth;
        public int HeightmapHeight;
        public Vector3 HeightMapScale;
        public Rect TerrainRect;
        public Vector3 TerrainPosition;
        public float MinHeightCells;

        public void Execute(int index)
        {
            Bounds cellBounds = CellBoundsList[index];
            Rect cellRect = RectExtension.CreateRectFromBounds(cellBounds);
            if (!TerrainRect.Overlaps(cellRect))
            {
                return;
            }

            var worldspaceCellCorner = new float2(cellBounds.center.x - cellBounds.extents.x,
                cellBounds.center.z - cellBounds.extents.z);
            var terrainspaceCellCorner = new float2(worldspaceCellCorner.x - TerrainPosition.x,
                worldspaceCellCorner.y - TerrainPosition.z);
            var heightmapPosition = new float2(terrainspaceCellCorner.x / HeightMapScale.x,
                terrainspaceCellCorner.y / HeightMapScale.z);

            var xCount = Mathf.CeilToInt(cellRect.width / HeightMapScale.x);
            var zCount = Mathf.CeilToInt(cellRect.height / HeightMapScale.z);

            var xStart = Mathf.FloorToInt(heightmapPosition.x);
            var zStart = Mathf.FloorToInt(heightmapPosition.y);

            var minHeight = float.MaxValue;
            var maxHeight = float.MinValue;
            for (var x = xStart; x <= xStart + xCount; x++)
            for (var z = zStart; z <= zStart + zCount; z++)
            {
                var heightSample = GetHeight(x, z);
                if (heightSample < minHeight)
                {
                    minHeight = heightSample;
                }

                if (heightSample > maxHeight)
                {
                    maxHeight = heightSample;
                }
            }

            if (maxHeight + TerrainPosition.y < MinHeightCells)
            {
                return;
            }

            var centerY = (maxHeight + minHeight) / 2f;
            var height = maxHeight - minHeight;
            cellBounds = new Bounds(
                new Vector3(cellBounds.center.x, centerY + TerrainPosition.y, cellBounds.center.z),
                new Vector3(cellBounds.size.x, height, cellBounds.size.z));
            CellBoundsList[index] = cellBounds;
        }

        private float GetHeight(int x, int y)
        {
            x = math.clamp(x, 0, HeightmapWidth - 1);
            y = math.clamp(y, 0, HeightmapHeight - 1);
            return InputHeights[y * HeightmapWidth + x] * HeightMapScale.y;
        }
    }
}
