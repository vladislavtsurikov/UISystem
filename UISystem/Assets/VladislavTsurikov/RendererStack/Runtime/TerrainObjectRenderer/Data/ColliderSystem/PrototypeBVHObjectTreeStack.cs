using System.Collections.Generic;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.RendererData;
using VladislavTsurikov.SceneDataSystem.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.ColliderSystem
{
    public class PrototypeBVHObjectTreeStack
    {
        public List<PrototypeBVHObjectTree> PrototypeBVHObjectTreeList = new();

        public void PreparePrototypeRenderCount(SelectionData selectionData)
        {
            foreach (Prototype prototype in selectionData.PrototypeList)
            {
                if (prototype == null || prototype.PrototypeConsole.HasError())
                {
                    continue;
                }

                if (GetPrototypeBVHObjectTree(prototype.ID) == null)
                {
                    PrototypeBVHObjectTreeList.Add(new PrototypeBVHObjectTree(prototype.ID));
                }
            }
        }

        public bool RegisterObject(TerrainObjectCollider terrainObjectCollider, Cell cell, ColliderCell colliderCell,
            PrototypeRendererData prototypeStorageRendererData, SceneDataManager sceneDataManager)
        {
            PrototypeBVHObjectTree prototypeBvhObjectTree =
                GetPrototypeBVHObjectTree(terrainObjectCollider.Instance.Proto.ID);

            if (prototypeBvhObjectTree != null)
            {
                prototypeBvhObjectTree.RegisterObject(terrainObjectCollider, cell, colliderCell,
                    prototypeStorageRendererData, sceneDataManager);
                return true;
            }

            return false;
        }

        public void ClearBvhObjectTree(int id)
        {
            PrototypeBVHObjectTree prototypeBVHObjectTree = GetPrototypeBVHObjectTree(id);
            if (prototypeBVHObjectTree != null)
            {
                prototypeBVHObjectTree.ClearBVHObjectTree();
            }
        }

        public void ClearBvhObjectTree()
        {
            foreach (PrototypeBVHObjectTree prototypeBvhObjectTree in PrototypeBVHObjectTreeList)
            {
                prototypeBvhObjectTree.ClearBVHObjectTree();
            }
        }

        public List<TerrainObjectCollider> GetAllInstances(int id)
        {
            PrototypeBVHObjectTree prototypeBvhObjectTree = GetPrototypeBVHObjectTree(id);

            if (prototypeBvhObjectTree == null)
            {
                return new List<TerrainObjectCollider>();
            }

            return prototypeBvhObjectTree.BVHObjectTree.FindAllInstance();
        }

        public PrototypeBVHObjectTree GetPrototypeBVHObjectTree(int id)
        {
            foreach (PrototypeBVHObjectTree item in PrototypeBVHObjectTreeList)
            {
                if (item.PrototypeID == id)
                {
                    return item;
                }
            }

            return null;
        }
    }
}
