using System;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.BVH.Runtime;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.QuadTree.Runtime;
using VladislavTsurikov.RendererStack.Runtime.Common;
using VladislavTsurikov.RendererStack.Runtime.Core;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.RendererData;
using VladislavTsurikov.SceneDataSystem.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.ColliderSystem
{
    public class TerrainObjectRendererCollider
    {
        public const int ConstCellSize = 50;
        private BvhCellTree<ColliderCell> _bvhCellTree;
        private QuadTree<ColliderCell> _cellQuadTree;

        [NonSerialized]
        public List<ColliderCell> CellList = new();

        public TerrainObjectRendererCollider()
        {
        }

        public TerrainObjectRendererCollider(SceneDataManager sceneDataManager, Cell cell,
            PrototypeRenderDataStack prototypeRenderDataStack)
        {
            RefreshCells(cell.Bounds);

            foreach (PrototypeRendererData prototypeRenderData in prototypeRenderDataStack.PrototypeRenderDataList)
            {
                var prototypeTerrainObject =
                    (PrototypeTerrainObject)TerrainObjectRenderer.Instance.SelectionData.GetProto(prototypeRenderData
                        .PrototypeID);

                foreach (RendererData.Instance instance in prototypeRenderData.InstanceList)
                {
                    var instantObject = new TerrainObjectInstance(instance.ID, instance.Position, instance.Scale,
                        instance.Rotation, prototypeTerrainObject);

                    RegisterObject(instantObject, cell, prototypeRenderData, sceneDataManager);
                }
            }
        }

        public void Setup()
        {
            var terrainObject =
                (TerrainObjectRenderer)RendererStackManager.Instance.RendererStack.GetElement(
                    typeof(TerrainObjectRenderer));

            if (terrainObject != null)
            {
                foreach (ColliderCell cell in CellList)
                {
                    cell.Prepare(this, terrainObject.SelectionData);
                }
            }
        }

        public void RemoveInstances(int id)
        {
            foreach (ColliderCell cell in CellList)
            {
                cell.PrototypeBVHObjectTreeStack.ClearBvhObjectTree(id);
                _bvhCellTree.ChangeNodeSize(cell, cell.GetObjectsAABB());
            }
        }

        public void RemoveInstances()
        {
            foreach (ColliderCell cell in CellList)
            {
                cell.PrototypeBVHObjectTreeStack.ClearBvhObjectTree();
                _bvhCellTree.ChangeNodeSize(cell, cell.GetObjectsAABB());
            }
        }

        public AABB GetAABB()
        {
            if (_bvhCellTree == null)
            {
                _bvhCellTree = new BvhCellTree<ColliderCell>();
            }

            return _bvhCellTree.GetAABB();
        }

        public List<TerrainObjectCollider> GetAllInstances()
        {
            var largeObjectColliders = new List<TerrainObjectCollider>();
            foreach (ColliderCell cell in CellList)
            foreach (PrototypeBVHObjectTree proto in cell.PrototypeBVHObjectTreeStack.PrototypeBVHObjectTreeList)
            {
                largeObjectColliders.AddRange(proto.BVHObjectTree.FindAllInstance());
            }

            return largeObjectColliders;
        }

        public void RefreshCells(Bounds bounds)
        {
            CellList.Clear();
            _bvhCellTree?.Clear();

            var cellSize = bounds.size.x;

            while (cellSize > ConstCellSize)
            {
                cellSize /= 2;
            }

            if (cellSize <= 0)
            {
                return;
            }

            var expandedBounds = new Bounds(bounds.center, bounds.size);
            expandedBounds.Expand(new Vector3(cellSize * 2f, 0, cellSize * 2f));

            Rect expandedRect = RectExtension.CreateRectFromBounds(expandedBounds);

            _cellQuadTree = new QuadTree<ColliderCell>(expandedRect);
            _bvhCellTree = new BvhCellTree<ColliderCell>();

            var cellXCount = Mathf.CeilToInt(bounds.size.x / cellSize);
            var cellZCount = Mathf.CeilToInt(bounds.size.z / cellSize);

            var corner = new Vector2(bounds.center.x - bounds.size.x / 2f,
                bounds.center.z - bounds.size.z / 2f);

            for (var x = 0; x <= cellXCount - 1; x++)
            for (var z = 0; z <= cellZCount - 1; z++)
            {
                var rect = new Rect(
                    new Vector2(cellSize * x + corner.x, cellSize * z + corner.y),
                    new Vector2(cellSize, cellSize));

                Bounds localBounds = RectExtension.CreateBoundsFromRect(rect, bounds.center.y, bounds.size.y);

                var cell = new ColliderCell(localBounds);

                CellList.Add(cell);
                _cellQuadTree.Insert(cell);
                _bvhCellTree.RegisterObject(cell, new AABB(cell.Bounds));
            }

            Setup();
        }

        public List<TerrainObjectCollider> OverlapBox(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation,
            ObjectFilter objectFilter, SceneData sceneData, Cell renderCell, bool quadTree, bool checkObbIntersection)
        {
            var overlappedObjects = new List<TerrainObjectCollider>();

            foreach (ColliderCell colliderCell in OverlapCellBox(boxCenter, boxSize, boxRotation, quadTree))
            foreach (PrototypeBVHObjectTree proto in
                     colliderCell.PrototypeBVHObjectTreeStack.PrototypeBVHObjectTreeList)
            {
                if (!proto.Filter(objectFilter))
                {
                    continue;
                }

                var pathDatas = new List<object>
                {
                    sceneData.SceneDataManager,
                    colliderCell,
                    renderCell,
                    renderCell.PrototypeRenderDataStack.GetPrototypeRenderData(proto.PrototypeID)
                };

                overlappedObjects.AddRange(proto.BVHObjectTree.OverlapBox(boxCenter, boxSize, boxRotation, null,
                    checkObbIntersection, pathDatas));
            }

            return overlappedObjects;
        }

        public List<TerrainObjectCollider> OverlapSphere(Vector3 sphereCenter, float sphereRadius,
            ObjectFilter objectFilter, SceneData sceneData, Cell renderCell, bool quadTree, bool checkOBBIntersection)
        {
            var overlappedObjects = new List<TerrainObjectCollider>();

            foreach (ColliderCell colliderCell in OverlapCellSphere(sphereCenter, sphereRadius, quadTree))
            foreach (PrototypeBVHObjectTree proto in
                     colliderCell.PrototypeBVHObjectTreeStack.PrototypeBVHObjectTreeList)
            {
                if (!proto.Filter(objectFilter))
                {
                    continue;
                }

                var pathDatas = new List<object>
                {
                    sceneData.SceneDataManager,
                    colliderCell,
                    renderCell,
                    renderCell.PrototypeRenderDataStack.GetPrototypeRenderData(proto.PrototypeID)
                };

                overlappedObjects.AddRange(proto.BVHObjectTree.OverlapSphere(sphereCenter, sphereRadius, null,
                    checkOBBIntersection, pathDatas));
            }

            return overlappedObjects;
        }

        public TerrainObjectCollider RegisterObject(TerrainObjectInstance instance, Cell cell,
            PrototypeRendererData prototypeStorageRendererData, SceneDataManager sceneDataManager)
        {
            var positionRect = new Rect(new Vector2(instance.Position.x, instance.Position.z), Vector2.zero);

            var overlapCellList = new List<ColliderCell>();
            _cellQuadTree.Query(positionRect, overlapCellList);

            if (overlapCellList.Count == 0)
            {
                return null;
            }

            var terrainObjectCollider = new TerrainObjectCollider(instance);

            ColliderCell colliderCell = overlapCellList[0];

            if (colliderCell.PrototypeBVHObjectTreeStack.RegisterObject(terrainObjectCollider, cell, colliderCell,
                    prototypeStorageRendererData, sceneDataManager))
            {
                ChangeNodeSizeIfNecessary(terrainObjectCollider, colliderCell);

                SceneObjectsBounds.ChangeSceneObjectsBounds(sceneDataManager.Sector);
            }

            return terrainObjectCollider;
        }

        public void ChangeNodeSizeIfNecessary(TerrainObjectCollider terrainObjectCollider, ColliderCell cell)
        {
            var cellAABB = new AABB(cell.Bounds);

            AABB newObjectsAABB = terrainObjectCollider.GetAABB();

            newObjectsAABB.Encapsulate(cellAABB);

            if (newObjectsAABB.Size.IsBigger(cellAABB.Size))
            {
                _bvhCellTree.ChangeNodeSize(cell, cell.GetObjectsAABB());
            }
        }

        public List<ColliderCell> OverlapCellBox(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation,
            bool quadTree)
        {
            var overlapCellList = new List<ColliderCell>();

            if (quadTree)
            {
                var position = new Vector2(boxCenter.x - boxSize.x / 2, boxCenter.z - boxSize.z / 2);
                var selectedAreaRect = new Rect(position, new Vector2(boxSize.x, boxSize.z));

                _cellQuadTree.Query(selectedAreaRect, overlapCellList);
            }
            else
            {
                overlapCellList = _bvhCellTree.OverlapCellsBox(boxCenter, boxSize, boxRotation);
            }

            return overlapCellList;
        }

        public List<ColliderCell> OverlapCellSphere(Vector3 sphereCenter, float sphereRadius, bool quadTree)
        {
            var overlapCellList = new List<ColliderCell>();

            if (quadTree)
            {
                var position = new Vector2(sphereCenter.x - sphereRadius, sphereCenter.z - sphereRadius);
                var selectedAreaRect = new Rect(position, new Vector2(sphereRadius * 2, sphereRadius * 2));

                _cellQuadTree.Query(selectedAreaRect, overlapCellList);
            }
            else
            {
                overlapCellList = _bvhCellTree.OverlapCellsSphere(sphereCenter, sphereRadius);
            }

            return overlapCellList;
        }

        public RayHit Raycast(Ray ray, ObjectFilter objectFilter, SceneData sceneData, Cell renderCell)
        {
            List<RayHit> allObjectHits = RaycastAll(ray, objectFilter, sceneData, renderCell);
            RayHit closestObjectHit = allObjectHits.Count != 0 ? allObjectHits[0] : null;
            return closestObjectHit;
        }

        public List<RayHit> RaycastAll(Ray ray, ObjectFilter objectFilter, SceneData sceneData, Cell renderCell)
        {
            List<ColliderCell> nodeHits = _bvhCellTree.RaycastAll(ray);

            var overlappedCells = new List<ColliderCell>();

            overlappedCells.AddRange(nodeHits);

            var rayHits = new List<RayHit>();

            foreach (ColliderCell colliderCell in overlappedCells)
            foreach (PrototypeBVHObjectTree proto in
                     colliderCell.PrototypeBVHObjectTreeStack.PrototypeBVHObjectTreeList)
            {
                if (!proto.Filter(objectFilter))
                {
                    continue;
                }

                var pathDatas = new List<object>
                {
                    sceneData.SceneDataManager,
                    colliderCell,
                    renderCell,
                    renderCell.PrototypeRenderDataStack.GetPrototypeRenderData(proto.PrototypeID)
                };

                foreach (RayHit rayHit in proto.BVHObjectTree.RaycastAll(ray, objectFilter, pathDatas))
                {
                    rayHits.Add(rayHit);
                }
            }

            return rayHits;
        }

#if UNITY_EDITOR
        public void DrawAllCells(Color nodeColor) => _bvhCellTree.DrawAllCells(nodeColor);

        public List<BVHNodeRayHit<ColliderCell>> DrawRaycast(Ray ray, Color nodeColor) =>
            _bvhCellTree.DrawRaycast(ray, nodeColor);
#endif
    }
}
