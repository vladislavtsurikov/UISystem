using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.ColliderSystem;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.RendererData;
using VladislavTsurikov.SceneDataSystem.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data
{
    public partial class TerrainObjectRendererData
    {
        public static TerrainObjectInstance AddInstance(TerrainObjectInstance instance, string sectorLayerTag)
        {
            List<SceneDataManager> sceneDataManagers =
                SceneDataManagerFinder.OverlapPosition(instance.Position, sectorLayerTag, false);

            if (sceneDataManagers.Count != 0)
            {
                return AddInstance(instance, sceneDataManagers[0]);
            }

            return null;
        }

        private static TerrainObjectInstance AddInstance(TerrainObjectInstance instance,
            SceneDataManager sceneDataManager)
        {
            var terrainObjectRendererData =
                (TerrainObjectRendererData)sceneDataManager.SceneDataStack.GetElement(
                    typeof(TerrainObjectRendererData));

            if (terrainObjectRendererData == null)
            {
                return null;
            }

            var positionRect = new Rect(new Vector2(instance.Position.x, instance.Position.z), Vector2.zero);

            var overlapCellList = new List<Cell>();
            terrainObjectRendererData._cellQuadTree.Query(positionRect, overlapCellList);

            if (overlapCellList.Count == 0)
            {
                return null;
            }

            Cell cell = overlapCellList[0];

            PrototypeRendererData prototypeRendererData =
                cell.PrototypeRenderDataStack.GetPrototypeRenderData(instance.Proto.ID);

            if (prototypeRendererData == null)
            {
                prototypeRendererData = new PrototypeRendererData(instance.Proto.ID);
                cell.PrototypeRenderDataStack.PrototypeRenderDataList.Add(prototypeRendererData);
            }

            TerrainObjectCollider instantObjectCollider =
                cell.TerrainObjectRendererCollider.RegisterObject(instance, cell, prototypeRendererData,
                    sceneDataManager);

            if (instantObjectCollider != null)
            {
                prototypeRendererData.AddPersistentData(instance);
                terrainObjectRendererData.ChangeNodeSizeIfNecessary(instantObjectCollider, cell);
                return instance;
            }

            return null;
        }

        public static void AddInstances(List<TerrainObjectCollider> instantObjects, string sectorLayerTag)
        {
            foreach (TerrainObjectCollider instance in instantObjects)
            {
                AddInstance(instance.Instance, sectorLayerTag);
            }
        }
    }
}
