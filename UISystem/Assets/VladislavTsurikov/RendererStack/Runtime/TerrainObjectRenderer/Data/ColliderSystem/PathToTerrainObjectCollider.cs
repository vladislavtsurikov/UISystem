using System.Collections.Generic;
using VladislavTsurikov.BVH.Runtime;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.RendererData;
using VladislavTsurikov.SceneDataSystem.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.ColliderSystem
{
    public class PathToTerrainObjectCollider : PathToColliderObject
    {
        public readonly BVHObjectTree<TerrainObjectCollider> BVHObjectTree;
        public readonly ColliderCell ColliderCell;
        public readonly BVHNodeAABB<TerrainObjectCollider> LeafNode;
        public readonly PrototypeRendererData PrototypeRendererData;
        public readonly Cell RenderCell;
        public readonly SceneDataManager SceneDataManager;

        public PathToTerrainObjectCollider(List<object> datas)
        {
            foreach (var item in datas)
            {
                switch (item)
                {
                    case SceneDataManager sceneDataManager:
                        SceneDataManager = sceneDataManager;
                        break;
                    case PrototypeRendererData data:
                        PrototypeRendererData = data;
                        break;
                    case ColliderCell cell:
                        ColliderCell = cell;
                        break;
                    case Cell cell:
                        RenderCell = cell;
                        break;
                    case BVHObjectTree<TerrainObjectCollider> tree:
                        BVHObjectTree = tree;
                        break;
                    case BVHNodeAABB<TerrainObjectCollider> node:
                        LeafNode = node;
                        break;
                }
            }
        }
    }
}
