#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using VladislavTsurikov.PhysicsSimulator.Runtime;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.BrushPhysicsTool
{
    public static class SpawnGroup
    {
        public static async UniTask SpawnGameObject(Group group, BoxArea area)
        {
            var scatterComponentSettings = (ScatterComponentSettings)group.GetElement(typeof(ScatterComponentSettings));
            scatterComponentSettings.ScatterStack.SetWaitingNextFrame(null);

            await scatterComponentSettings.ScatterStack.Samples(area, sample =>
            {
                RayHit rayHit = RaycastUtility.Raycast(
                    RayUtility.GetRayDown(new Vector3(sample.x, area.Center.y, sample.y)),
                    GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(group.PrototypeType));

                if (rayHit != null)
                {
                    var proto =
                        (PrototypeGameObject)GetRandomPrototype.GetMaxSuccessProto(group.GetAllSelectedPrototypes());

                    if (proto == null || proto.Active == false)
                    {
                        return;
                    }

                    SpawnPrototype.SpawnGameObject(group, proto, area, rayHit);
                }
            });

            RandomUtility.ChangeRandomSeed();
        }

#if RENDERER_STACK
        public static async UniTask SpawnTerrainObject(Group group, BoxArea area)
        {
            ScriptingSystem.SetColliders(new Sphere(area.Center, area.Size.x / 2 * 5), area);

            var scatterComponentSettings =
                (ScatterComponentSettings)group.GetElement(typeof(ScatterComponentSettings));
            scatterComponentSettings.ScatterStack.SetWaitingNextFrame(null);

            await scatterComponentSettings.ScatterStack.Samples(area, sample =>
            {
                RayHit rayHit =
                    RaycastUtility.Raycast(RayUtility.GetRayDown(new Vector3(sample.x, area.Center.y, sample.y)),
                        GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(
                            group.PrototypeType));

                if (rayHit != null)
                {
                    var proto =
                        (PrototypeTerrainObject)GetRandomPrototype.GetMaxSuccessProto(group.GetAllSelectedPrototypes());

                    if (proto == null || proto.Active == false)
                    {
                        return;
                    }

                    SpawnPrototype.SpawnTerrainObject(group, proto, area, rayHit);
                }
            });

            RandomUtility.ChangeRandomSeed();

            await UniTask.WaitUntil(() => SimulatedBodyStack.Count == 0);

            ScriptingSystem.RemoveColliders(area);
        }
#endif
    }
}
#endif
