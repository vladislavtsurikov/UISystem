using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Profiling;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.QuadTree.Runtime;
using VladislavTsurikov.RendererStack.Runtime.Common.TerrainSystem;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.ColliderSystem;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.RendererData;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.SceneSettings.Camera;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data
{
    public partial class TerrainObjectRendererData
    {
        private static List<Cell> cellsCached = new();

        private static List<Cell> visibleCellListCached = new();

        public static List<Cell> GetAllVisibleCellList(VirtualCamera virtualCamera)
        {
            if (cellsCached == null)
            {
                cellsCached = new List<Cell>();
            }

            cellsCached.Clear();

            Profiler.BeginSample("GetAllVisibleCellList");
            foreach (TerrainObjectRendererData item in
                     SceneDataStackUtility.GetAllSceneData<TerrainObjectRendererData>())
            {
                cellsCached.AddRange(item.GetVisibleCellList(virtualCamera));
            }

            Profiler.EndSample();

            return cellsCached;
        }

        public static List<Cell> GetCellsWithInstances(List<Cell> cellList, int protoID, out int totalInstanceCount)
        {
            totalInstanceCount = 0;
            var currentCellList = new List<Cell>();

            foreach (Cell cell in cellList)
            {
                PrototypeRendererData prototypeRendererData =
                    cell.PrototypeRenderDataStack.GetPrototypeRenderData(protoID);
                var count = prototypeRendererData?.InstanceList.Count ?? 0;

                if (count != 0)
                {
                    currentCellList.Add(cell);
                    totalInstanceCount += count;
                }
            }

            return currentCellList;
        }

        public static void RefreshAllCells()
        {
            var largeObjectInstances = new List<TerrainObjectCollider>();

            SceneDataStackUtility.Setup<TerrainObjectRendererData>(true);

            foreach (TerrainObjectRendererData item in
                     SceneDataStackUtility.GetAllSceneData<TerrainObjectRendererData>())
            {
                largeObjectInstances.AddRange(item.GetAllInstances());

                item.RefreshCells();
            }

            foreach (TerrainObjectCollider instance in largeObjectInstances)
            {
                AddInstance(instance.Instance, Sectorize.Sectorize.GetSectorLayerTag());
            }
        }

        public List<Cell> GetVisibleCellList(VirtualCamera virtualCamera)
        {
            var terrainObjectRendererCameraSettings =
                (TerrainObjectRendererCameraSettings)virtualCamera.CameraComponentStack.GetElement(
                    typeof(TerrainObjectRendererCameraSettings));

            if (visibleCellListCached == null)
            {
                visibleCellListCached = new List<Cell>();
            }

            visibleCellListCached.Clear();

            if (terrainObjectRendererCameraSettings == null)
            {
                return visibleCellListCached;
            }

            var frustumPlaneArray = new Plane[6];

            GeometryUtility.CalculateFrustumPlanes(virtualCamera.Camera, frustumPlaneArray);

            var maxDistance = virtualCamera.GetMaxDistance(typeof(TerrainObjectRenderer));

            Vector3 selectedCameraPosition = virtualCamera.GetCameraPosition();

            var areaSize = maxDistance * 2 + _cellSize;

            var position = new Vector2(selectedCameraPosition.x - areaSize / 2f,
                selectedCameraPosition.z - areaSize / 2f);
            var selectedAreaRect = new Rect(position, new Vector2(areaSize, areaSize));

            if (_cellQuadTree == null)
            {
                return visibleCellListCached;
            }

            Profiler.BeginSample("QueryCells");

            //_cellQuadTree.Query(selectedAreaRect, visibleCellListCached);

            _cellQuadTree.Query(selectedAreaRect, cell =>
            {
                // if(Vector3.Distance(selectedCameraPosition, cell.Bounds.center) > maxDistance + cell.Bounds.size.x)
                // {
                //     return true;
                // }

                if (terrainObjectRendererCameraSettings.CameraCullingMode == CameraCullingMode.FrustumCulling)
                {
                    if (GeometryUtility.TestPlanesAABB(frustumPlaneArray, cell.GetObjectBounds()))
                    {
                        visibleCellListCached.Add(cell);
                    }
                }
                else
                {
                    visibleCellListCached.Add(cell);
                }

                return true;
            });

            Profiler.EndSample();

            return visibleCellListCached;
        }

        public void RefreshCells()
        {
            ModifiedPrototypeRenderDataStack.Clear();

            TerrainManager terrainManager =
                SceneDataStackUtility.InstanceSceneData<TerrainManager>(SceneDataManager.Scene);
            terrainManager.Setup(true);

            CellList.Clear();
            BVHCellTree.Clear();

            if (terrainManager.Bounds.size == Vector3.zero)
            {
                return;
            }

            _cellSize = terrainManager.Bounds.size.x;

            while (_cellSize > ConstCellSize)
            {
                _cellSize /= 2;
            }

            if (_cellSize <= 0)
            {
                return;
            }

            _bounds = new Bounds(terrainManager.Bounds.center, terrainManager.Bounds.size);
            Bounds expandedBounds = _bounds;
            expandedBounds.Expand(new Vector3(_cellSize * 2f, 0, _cellSize * 2f));

            Rect expandedRect = RectExtension.CreateRectFromBounds(expandedBounds);

            _cellQuadTree = new QuadTree<Cell>(expandedRect);

            var cellXCount = Mathf.CeilToInt(terrainManager.Bounds.size.x / _cellSize);
            var cellZCount = Mathf.CeilToInt(terrainManager.Bounds.size.z / _cellSize);

            var corner = new Vector2(terrainManager.Bounds.center.x - terrainManager.Bounds.size.x / 2f,
                terrainManager.Bounds.center.z - terrainManager.Bounds.size.z / 2f);

            for (var x = 0; x <= cellXCount - 1; x++)
            for (var z = 0; z <= cellZCount - 1; z++)
            {
                var cell = new Cell(new Rect(
                    new Vector2(_cellSize * x + corner.x, _cellSize * z + corner.y),
                    new Vector2(_cellSize, _cellSize)), this, TerrainObjectRenderer.Instance.SelectionData);
                CellList.Add(cell);
                cell.Index = CellList.Count - 1;
                _cellQuadTree.Insert(cell);
            }

            var cellBounds =
                new NativeArray<Bounds>(CellList.Count, Allocator.Persistent);
            for (var i = 0; i <= CellList.Count - 1; i++)
            {
                cellBounds[i] = CellList[i].Bounds;
            }

            var minBoundsHeight = terrainManager.Bounds.center.y - terrainManager.Bounds.extents.y;

            var minHeightCells = minBoundsHeight;

            JobHandle jobHandle = default;

            for (var i = 0; i <= terrainManager.TerrainHelperList.Count - 1; i++)
            {
                jobHandle = terrainManager.TerrainHelperList[i]
                    .SetCellHeight(cellBounds, minHeightCells, expandedRect, jobHandle);
            }

            jobHandle.Complete();

            for (var i = 0; i <= CellList.Count - 1; i++)
            {
                CellList[i].Bounds = cellBounds[i];
            }

            cellBounds.Dispose();

            foreach (Cell cell in CellList)
            {
                BVHCellTree.RegisterObject(cell, new AABB(cell.Bounds));
                cell.TerrainObjectRendererCollider.RefreshCells(cell.Bounds);
            }
        }
    }
}
