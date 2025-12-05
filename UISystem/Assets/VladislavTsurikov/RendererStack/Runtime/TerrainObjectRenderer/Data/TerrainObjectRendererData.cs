using System;
using System.Collections.Generic;
using OdinSerializer;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.QuadTree.Runtime;
using VladislavTsurikov.RendererStack.Runtime.Common;
using VladislavTsurikov.RendererStack.Runtime.Common.TerrainSystem;
using VladislavTsurikov.RendererStack.Runtime.Core;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.ColliderSystem;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.RendererData;
using VladislavTsurikov.SceneDataSystem.Runtime;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data
{
    [AllowCreateComponentWithTerrains]
    public partial class TerrainObjectRendererData : RendererSceneData, IRaycast
    {
        private const int ConstCellSize = 500;

        [OdinSerialize]
        private Bounds _bounds;

        [NonSerialized]
        private QuadTree<Cell> _cellQuadTree;

        [OdinSerialize]
        private float _cellSize;

        [NonSerialized]
        public BvhCellTree<Cell> BVHCellTree = new();

        [OdinSerialize]
        public List<Cell> CellList = new();

        protected override void SetupSceneData()
        {
            if (CellList.Count == 0)
            {
                RefreshCells();
                return;
            }

            if (RendererStackManager.Instance == null)
            {
                return;
            }

            var terrainObject =
                (TerrainObjectRenderer)RendererStackManager.Instance.RendererStack.GetElement(
                    typeof(TerrainObjectRenderer));
            terrainObject.ForceUpdateRendererData = true;

            SetupNonSerializedData();

            foreach (Cell cell in CellList)
            {
                cell.Setup(this, terrainObject.SelectionData);
            }
        }

        public override AABB GetAABB()
        {
            SetupNonSerializedData();

            return BVHCellTree.GetAABB();
        }

        protected override void OnDisableElement()
        {
            for (var i = 0; i <= CellList.Count - 1; i++)
            {
                CellList[i].PrototypeRenderDataStack.OnDisable();
            }
        }

        private void SetupNonSerializedData()
        {
            if (_cellQuadTree == null || BVHCellTree == null)
            {
                Bounds expandedBounds = _bounds;
                expandedBounds.Expand(new Vector3(_cellSize * 2f, 0, _cellSize * 2f));

                Rect expandedRect = RectExtension.CreateRectFromBounds(expandedBounds);

                _cellQuadTree = new QuadTree<Cell>(expandedRect);
                BVHCellTree = new BvhCellTree<Cell>();

                foreach (Cell cell in CellList)
                {
                    _cellQuadTree.Insert(cell);
                    BVHCellTree.RegisterObject(cell, new AABB(cell.Bounds));
                }
            }
        }

        public void RemoveInstances(int prototypeID)
        {
            ScriptingSystem.ScriptingSystem.RemoveCollider(prototypeID);

            foreach (Cell cell in CellList)
            {
                PrototypeRendererData prototypeRendererData =
                    cell.PrototypeRenderDataStack.GetPrototypeRenderData(prototypeID);

                if (prototypeRendererData == null || prototypeRendererData.InstanceList.Count == 0)
                {
                    continue;
                }

                prototypeRendererData.ClearPersistentData();

                cell.PrototypeRenderDataStack.RemoveInstances(prototypeID);
                cell.TerrainObjectRendererCollider.RemoveInstances(prototypeID);

                BVHCellTree.ChangeNodeSize(cell, cell.GetObjectsAABB());
            }
        }

        public List<TerrainObjectCollider> GetAllInstances()
        {
            var largeObjectColliders = new List<TerrainObjectCollider>();

            foreach (Cell cell in CellList)
            {
                largeObjectColliders.AddRange(cell.TerrainObjectRendererCollider.GetAllInstances());
            }

            return largeObjectColliders;
        }

        public void ChangeNodeSizeIfNecessary(TerrainObjectCollider terrainObjectCollider, Cell cell)
        {
            if (terrainObjectCollider == null)
            {
                return;
            }

            var cellAABB = new AABB(cell.Bounds);

            AABB newObjectsAABB = terrainObjectCollider.GetAABB();

            newObjectsAABB.Encapsulate(cellAABB);

            if (!cell.InitialObjectsAABB.IsValid || newObjectsAABB.Size.IsBigger(cell.InitialObjectsAABB.Size))
            {
                BVHCellTree.ChangeNodeSize(cell, cell.GetObjectsAABB());
                cell.InitialObjectsAABB = cell.GetObjectsAABB();
            }
        }

#if UNITY_EDITOR
        public override void DrawDebug()
        {
            if (!IsSetup)
            {
                return;
            }

            var cameraManager =
                (CameraManager)RendererStackManager.Instance.SceneComponentStack.GetElement(typeof(CameraManager));

            TerrainObjectRenderer terrainObjectRenderer = TerrainObjectRenderer.Instance;

            if (terrainObjectRenderer.DebugAllCells)
            {
                Handles.color = Color.blue;

                foreach (Cell cell in CellList)
                {
                    Bounds bounds = cell.GetObjectBounds();

                    Handles.DrawWireCube(bounds.center, bounds.size);
                }
            }

            if (terrainObjectRenderer.DebugVisibleCells)
            {
                foreach (VirtualCamera cam in cameraManager.VirtualCameraList)
                {
                    if (cam.Ignored)
                    {
                        continue;
                    }

                    List<Cell> visibleCellList = GetVisibleCellList(cam);

                    for (var i = 0; i < visibleCellList.Count; i++)
                    {
                        Bounds bounds = visibleCellList[i].GetObjectBounds();

                        Handles.color = Color.green;
                        Handles.DrawWireCube(bounds.center, bounds.size);
                    }
                }
            }
        }
#endif
    }
}
