using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter.Settings.ScatterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using VladislavTsurikov.PhysicsSimulator.Runtime;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem;

namespace VladislavTsurikov.MegaWorld.Runtime.GravitySpawner.Utility
{
    public static class SpawnGroup
    {
        public static async UniTask SpawnGameObject(CancellationToken token, GravitySpawner gravitySpawner, Group group,
            TerrainsMaskManager terrainsMaskManager, BoxArea area)
        {
            ScriptingSystem.SetColliders(new Sphere(area.Center, area.Size.x / 2), area);

            var scatterComponentSettings = (ScatterComponentSettings)group.GetElement(typeof(ScatterComponentSettings));
            scatterComponentSettings.ScatterStack.SetWaitingNextFrame(new PhysicsWaitingNextFrame(1000));

            await scatterComponentSettings.ScatterStack.Samples(area, sample =>
            {
                RayHit rayHit = RaycastUtility.Raycast(
                    RayUtility.GetRayDown(new Vector3(sample.x, area.Center.y, sample.y)),
                    GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(group.PrototypeType));

                if (rayHit != null)
                {
                    var proto = (PrototypeGameObject)GetRandomPrototype.GetMaxSuccessProto(group.PrototypeList);

                    if (proto == null || proto.Active == false)
                    {
                        return;
                    }

                    SpawnPrototype.SpawnGameObject(gravitySpawner, group, proto, terrainsMaskManager, area, rayHit);
                }
            }, token);

            while (!IsDone())
            {
                token.ThrowIfCancellationRequested();
#if UNITY_EDITOR
                gravitySpawner.UpdateDisplayProgressBar("Running",
                    "Running " + group.name + " (simulated objects left: " + SimulatedBodyStack.Count + ")");
#endif

                await UniTask.Yield();
            }

            bool IsDone()
            {
                return SimulatedBodyStack.Count == 0;
            }

            ScriptingSystem.RemoveColliders(area);
        }

#if RENDERER_STACK
        public static async UniTask SpawnTerrainObject(CancellationToken token, GravitySpawner gravitySpawner,
            Group group, TerrainsMaskManager terrainsMaskManager, BoxArea area)
        {
            ScriptingSystem.SetColliders(new Sphere(area.Center, area.Size.x / 2), area);

            var scatterComponentSettings =
                (ScatterComponentSettings)group.GetElement(typeof(ScatterComponentSettings));
            scatterComponentSettings.ScatterStack.SetWaitingNextFrame(new PhysicsWaitingNextFrame(1000));

            await scatterComponentSettings.ScatterStack.Samples(area, sample =>
            {
                RayHit rayHit =
                    RaycastUtility.Raycast(RayUtility.GetRayDown(new Vector3(sample.x, area.Center.y, sample.y)),
                        GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(
                            group.PrototypeType));
                if (rayHit != null)
                {
                    var proto =
                        (PrototypeTerrainObject)GetRandomPrototype.GetMaxSuccessProto(group.PrototypeList);

                    if (proto == null || proto.Active == false)
                    {
                        return;
                    }

                    SpawnPrototype.SpawnTerrainObject(gravitySpawner, group, proto, terrainsMaskManager, area, rayHit);
                }
            }, token);

            while (!IsDone())
            {
                token.ThrowIfCancellationRequested();
#if UNITY_EDITOR
                gravitySpawner.UpdateDisplayProgressBar("Running",
                    "Running " + group.name + " (simulated objects left: " + SimulatedBodyStack.Count + ")");
#endif

                await UniTask.Yield();
            }

            bool IsDone()
            {
                return SimulatedBodyStack.Count == 0;
            }

            ScriptingSystem.RemoveColliders(area);
        }
#endif
    }
}
