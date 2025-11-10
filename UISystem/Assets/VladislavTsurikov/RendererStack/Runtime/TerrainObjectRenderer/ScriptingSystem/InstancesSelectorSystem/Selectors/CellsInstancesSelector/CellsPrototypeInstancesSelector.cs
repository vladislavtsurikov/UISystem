using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.ColliderSystem;
using VladislavTsurikov.SceneDataSystem.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.InstancesSelectorSystem.
    CellsInstancesSelector
{
    public class CellsPrototypeInstancesSelector : PrototypeInstancesSelector
    {
        private readonly PrototypeTerrainObject _proto;

        private readonly ScriptingSystemPool _scriptingSystemPool;

        public CellsPrototypeInstancesSelector(PrototypeScriptingManager prototypeScriptingManager,
            PrototypeTerrainObject proto) : base(proto)
        {
            _scriptingSystemPool = new ScriptingSystemPool(prototypeScriptingManager);
            _proto = proto;
        }

        public override void OnNewInstanceVisibility(TerrainObjectInstance instance) =>
            _scriptingSystemPool.Get(instance);

        public override void OnInstanceInvisible(TerrainObjectInstance instance)
        {
            if (instance.HierarchyTerrainObjectInstance == null)
            {
                return;
            }

            _scriptingSystemPool.Release(instance.HierarchyTerrainObjectInstance.gameObject);
            instance.HierarchyTerrainObjectInstance = null;
        }

        protected override void OnDisableCollider(PrototypeInstancesSelectorData prototypeInstancesSelectorData,
            object usedObj)
        {
            var cellPrototypeInstancesSelectorData = (CellPrototypeInstancesSelectorData)prototypeInstancesSelectorData;

            foreach (ColliderCell colliderCell in cellPrototypeInstancesSelectorData.LastColliderCellList)
            foreach (TerrainObjectCollider largeObjectCollider in colliderCell.PrototypeBVHObjectTreeStack
                         .GetAllInstances(_proto.ID))
            {
                DisableCollider(largeObjectCollider, usedObj);
            }
        }

        protected override void OnDisableCollider(PrototypeInstancesSelectorData prototypeInstancesSelectorData)
        {
            var cellPrototypeInstancesSelectorData = (CellPrototypeInstancesSelectorData)prototypeInstancesSelectorData;

            foreach (ColliderCell colliderCell in cellPrototypeInstancesSelectorData.LastColliderCellList)
            foreach (TerrainObjectCollider largeObjectCollider in colliderCell.PrototypeBVHObjectTreeStack
                         .GetAllInstances(_proto.ID))
            {
                DisableCollider(largeObjectCollider);
            }
        }

        public override Type GetInstanceSelectorData() => typeof(CellPrototypeInstancesSelectorData);

        public override void SetColliders(Sphere sphere, object usedObj)
        {
            var cellPrototypeInstancesSelectorData =
                (CellPrototypeInstancesSelectorData)GetPrototypeInstancesSelectorData(usedObj);

            Profiler.BeginSample("ColliderUtility.OverlapSphere");

            List<ColliderCell> newColliderCells = OverlapColliderCell(sphere.Center, sphere.Radius);

            Profiler.EndSample();

            Profiler.BeginSample("After finding objects");

            foreach (ColliderCell colliderCell in newColliderCells)
            {
                if (cellPrototypeInstancesSelectorData.LastColliderCellList.Contains(colliderCell))
                {
                    cellPrototypeInstancesSelectorData.LastColliderCellList.Remove(colliderCell);
                    continue;
                }

                cellPrototypeInstancesSelectorData.LastColliderCellList.Remove(colliderCell);

                foreach (TerrainObjectCollider largeObjectCollider in colliderCell.PrototypeBVHObjectTreeStack
                             .GetAllInstances(_proto.ID))
                {
                    if (largeObjectCollider.Instance.HierarchyTerrainObjectInstance == null)
                    {
                        EnableCollider(largeObjectCollider, usedObj);
                    }
                }
            }

            foreach (ColliderCell colliderCell in cellPrototypeInstancesSelectorData.LastColliderCellList)
            foreach (TerrainObjectCollider largeObjectCollider in colliderCell.PrototypeBVHObjectTreeStack
                         .GetAllInstances(_proto.ID))
            {
                DisableCollider(largeObjectCollider, usedObj);
            }

            cellPrototypeInstancesSelectorData.LastColliderCellList.Clear();
            cellPrototypeInstancesSelectorData.LastColliderCellList = newColliderCells;

            Profiler.EndSample();
        }

        private List<ColliderCell> OverlapColliderCell(Vector3 sphereCenter, float sphereRadius)
        {
            List<SceneDataManager> sectorList =
                SceneDataManagerFinder.OverlapSphere(sphereCenter, sphereRadius,
                    Sectorize.Sectorize.GetSectorLayerTag());

            var colliderCells = new List<ColliderCell>();

            foreach (SceneDataManager node in sectorList)
            {
                var terrainObjectRendererData =
                    (TerrainObjectRendererData)node.SceneDataStack.GetElement(typeof(TerrainObjectRendererData));

                if (terrainObjectRendererData != null)
                {
                    foreach (Cell cell in
                             terrainObjectRendererData.OverlapCellSphere(sphereCenter, sphereRadius, false))
                    {
                        colliderCells.AddRange(
                            cell.TerrainObjectRendererCollider.OverlapCellSphere(sphereCenter, sphereRadius, false));
                    }
                }
            }

            return colliderCells;
        }
    }
}
