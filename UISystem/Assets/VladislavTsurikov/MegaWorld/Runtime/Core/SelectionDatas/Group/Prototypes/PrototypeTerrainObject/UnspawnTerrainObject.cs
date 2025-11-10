using System.Collections.Generic;
using UnityEngine;
#if RENDERER_STACK
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer;
using VladislavTsurikov.UnityUtility.Runtime;
#endif

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject
{
    public static class UnspawnTerrainObject
    {
        public static void Unspawn(IReadOnlyList<Prototype> prototypes, bool unspawnSelected)
        {
#if RENDERER_STACK
            var unspawnPrefabs = new List<GameObject>();

            foreach (PrototypeTerrainObject proto in prototypes)
            {
                if (unspawnSelected)
                {
                    if (proto.Selected == false)
                    {
                        continue;
                    }
                }

                unspawnPrefabs.Add((GameObject)proto.PrototypeObject);

                TerrainObjectRendererAPI.RemoveInstances(proto.RendererPrototypeID);
            }

            //Not a necessary call, but the tools can spawn GameObjects to later convert them into Terrain Object Renderer
            GameObjectUtility.Unspawn(unspawnPrefabs);
#endif
        }
    }
}
