using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainDetail;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Utility;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Utility.Spawn
{
    public static class SpawnGroup
    {
        public static void SpawnTerrainDetails(Group group, CallbackList<Prototype> protoTerrainDetailList,
            BoxArea boxArea)
        {
            if (TerrainResourcesController.IsSyncError(group, Terrain.activeTerrain))
            {
                return;
            }

            UpdateFilterMask.UpdateFilterMaskForPrototypes(protoTerrainDetailList, boxArea);

            foreach (Terrain terrain in Terrain.activeTerrains)
            {
                var terrainBounds = new Bounds(terrain.terrainData.bounds.center + terrain.transform.position,
                    terrain.terrainData.bounds.size);

                if (terrainBounds.Intersects(boxArea.Bounds))
                {
                    if (terrain.terrainData.detailPrototypes.Length == 0)
                    {
                        Debug.LogWarning("Add Terrain Details");
                        return;
                    }

                    foreach (PrototypeTerrainDetail proto in protoTerrainDetailList)
                    {
                        if (proto.Active == false)
                        {
                            continue;
                        }

                        SpawnPrototype.SpawnTerrainDetails(proto, boxArea, terrain);
                    }
                }
            }
        }

        public static void SpawnTerrainTexture(Group group, IReadOnlyList<Prototype> prototypeTerrainTextures,
            BoxArea boxArea, float textureTargetStrength)
        {
            if (TerrainResourcesController.IsSyncError(group, Terrain.activeTerrain))
            {
                return;
            }

            if (boxArea.TerrainUnder == null)
            {
                return;
            }

            var terrainPainterRenderHelper = new TerrainPainterRenderHelper(boxArea);

            foreach (PrototypeTerrainTexture proto in prototypeTerrainTextures)
            {
                if (proto.Active)
                {
                    SpawnPrototype.SpawnTexture(proto, terrainPainterRenderHelper, textureTargetStrength);
                }
            }
        }
    }
}
