using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainDetail;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;

namespace VladislavTsurikov.MegaWorld.Runtime.TerrainSpawner.Utility
{
    public static class SpawnGroup
    {
        public static async UniTask SpawnGameObject(CancellationToken token, Group group,
            TerrainsMaskManager terrainsMaskManager, BoxArea boxArea, bool displayProgressBar)
        {
            var scatterComponentSettings = (ScatterComponentSettings)group.GetElement(typeof(ScatterComponentSettings));

            scatterComponentSettings.ScatterStack.SetWaitingNextFrame(displayProgressBar
                ? new DefaultWaitingNextFrame(0.2f)
                : null);

            LayerSettings layerSettings = GlobalCommonComponentSingleton<LayerSettings>.Instance;

            await scatterComponentSettings.ScatterStack.Samples(boxArea, sample =>
            {
                RayHit rayHit =
                    RaycastUtility.Raycast(
                        RayUtility.GetRayDown(new Vector3(sample.x, boxArea.RayHit.Point.y, sample.y)),
                        layerSettings.GetCurrentPaintLayers(group.PrototypeType));
                if (rayHit != null)
                {
                    var proto = (PrototypeGameObject)GetRandomPrototype.GetMaxSuccessProto(group.PrototypeList);
                    ;

                    if (proto == null || proto.Active == false)
                    {
                        return;
                    }

                    var fitness = GetFitness.Get(group, terrainsMaskManager, rayHit);

                    if (fitness != 0)
                    {
                        if (Random.Range(0f, 1f) < 1 - fitness)
                        {
                            return;
                        }

                        Common.Utility.Spawn.SpawnPrototype.SpawnGameObject(group, proto, rayHit, fitness);
                    }
                }
            }, token);
        }

#if RENDERER_STACK
        public static async UniTask SpawnTerrainObject(CancellationToken token, Group group,
            TerrainsMaskManager terrainsMaskManager, BoxArea boxArea, bool displayProgressBar)
        {
            var scatterComponentSettings =
                (ScatterComponentSettings)group.GetElement(typeof(ScatterComponentSettings));

            scatterComponentSettings.ScatterStack.SetWaitingNextFrame(displayProgressBar
                ? new DefaultWaitingNextFrame(0.2f)
                : null);

            LayerSettings layerSettings = GlobalCommonComponentSingleton<LayerSettings>.Instance;

            await UniTask.Delay(1000);

            await scatterComponentSettings.ScatterStack.Samples(boxArea, sample =>
            {
                RayHit rayHit =
                    RaycastUtility.Raycast(
                        RayUtility.GetRayDown(new Vector3(sample.x, boxArea.RayHit.Point.y, sample.y)),
                        layerSettings.GetCurrentPaintLayers(group.PrototypeType));
                if (rayHit != null)
                {
                    var proto =
                        (PrototypeTerrainObject)GetRandomPrototype.GetMaxSuccessProto(group.PrototypeList);

                    if (proto == null || proto.Active == false)
                    {
                        return;
                    }

                    var fitness = GetFitness.Get(group, terrainsMaskManager, rayHit);

                    if (fitness != 0)
                    {
                        if (Random.Range(0f, 1f) < 1 - fitness)
                        {
                            return;
                        }

                        Common.Utility.Spawn.SpawnPrototype.SpawnTerrainObject(proto, rayHit, fitness);
                    }
                }
            }, token);
        }
#endif

        public static async UniTask SpawnTerrainDetails(CancellationToken token, Group group,
            IReadOnlyList<Prototype> protoTerrainDetailList, TerrainsMaskManager terrainsMaskManager, BoxArea boxArea)
        {
            if (TerrainResourcesController.IsSyncError(group, Terrain.activeTerrain))
            {
                return;
            }

            foreach (Terrain terrain in Terrain.activeTerrains)
            {
                var terrainBounds = new Bounds(terrain.terrainData.bounds.center + terrain.transform.position,
                    terrain.terrainData.bounds.size);

                if (terrainBounds.Intersects(boxArea.Bounds))
                {
                    if (terrain.terrainData.detailPrototypes.Length == 0)
                    {
                        Debug.LogWarning("Add Terrain Details");
                        continue;
                    }

                    foreach (PrototypeTerrainDetail proto in protoTerrainDetailList)
                    {
                        if (proto.Active == false)
                        {
                            continue;
                        }

                        SpawnPrototype.SpawnTerrainDetails(group, proto, terrainsMaskManager, boxArea, terrain);

                        await UniTask.Yield();
                    }

                    await UniTask.Yield();
                }
            }
        }
    }
}
