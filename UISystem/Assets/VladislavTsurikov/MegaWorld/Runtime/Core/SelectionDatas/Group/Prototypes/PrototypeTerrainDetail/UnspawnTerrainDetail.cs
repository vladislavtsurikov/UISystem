using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainDetail
{
    public static class UnspawnTerrainDetail
    {
        public static void Unspawn(IReadOnlyList<Prototype> protoTerrainDetailList, bool unspawnSelected)
        {
            foreach (PrototypeTerrainDetail prototype in protoTerrainDetailList)
            {
                if (unspawnSelected)
                {
                    if (prototype.Selected == false)
                    {
                        continue;
                    }
                }

                foreach (Terrain terrain in Terrain.activeTerrains)
                {
                    if (terrain.terrainData.detailResolution == 0)
                    {
                        continue;
                    }

                    if (prototype.TerrainProtoId > terrain.terrainData.detailPrototypes.Length - 1)
                    {
                        Debug.LogWarning(
                            "You need all Terrain Details prototypes to be in the terrain. Click \"Add Missing Resources To Terrain\"");
                    }
                    else
                    {
                        terrain.terrainData.SetDetailLayer(0, 0, prototype.TerrainProtoId,
                            new int[terrain.terrainData.detailWidth, terrain.terrainData.detailWidth]);
                    }
                }
            }
        }
    }
}
