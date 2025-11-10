#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.PhysicsSimulator.Runtime;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.ExplodePhysics
{
    public static class SpawnGroup
    {
        public static void SpawnGameObject(Group group, RayHit rayHit)
        {
            var settings = (ExplodePhysicsToolSettings)ToolsComponentStack.GetElement(typeof(ExplodePhysicsTool),
                typeof(ExplodePhysicsToolSettings));

            var spawnCount = Random.Range(settings.InstancesMin, settings.InstancesMax);

            for (var i = 0; i < spawnCount; i++)
            {
                var proto =
                    (PrototypeGameObject)GetRandomPrototype.GetMaxSuccessProto(group.GetAllSelectedPrototypes());

                Vector3 centerPosition = rayHit.Point + new Vector3(0, settings.PositionOffsetY, 0);
                Vector3 positionSpawn = centerPosition + Random.insideUnitSphere * (settings.Size / 2);

                if (settings.SpawnFromOnePoint)
                {
                    positionSpawn = centerPosition;
                }

                SpawnPrototype.SpawnGameObject(group, proto, settings, rayHit, positionSpawn, centerPosition);
            }

            RandomUtility.ChangeRandomSeed();
        }

#if RENDERER_STACK
        public static async UniTask SpawnTerrainObject(Group group, RayHit rayHit)
        {
            ScriptingSystem.SetColliders(new Sphere(rayHit.Point, 500), rayHit);

            var settings =
                (ExplodePhysicsToolSettings)ToolsComponentStack.GetElement(typeof(ExplodePhysicsTool),
                    typeof(ExplodePhysicsToolSettings));

            var spawnCount = Random.Range(settings.InstancesMin, settings.InstancesMax);

            for (var i = 0; i < spawnCount; i++)
            {
                var proto =
                    (PrototypeTerrainObject)GetRandomPrototype.GetMaxSuccessProto(group.GetAllSelectedPrototypes());

                Vector3 centerPosition = rayHit.Point + new Vector3(0, settings.PositionOffsetY, 0);
                Vector3 positionSpawn = centerPosition + Random.insideUnitSphere * (settings.Size / 2);

                if (settings.SpawnFromOnePoint)
                {
                    positionSpawn = centerPosition;
                }

                SpawnPrototype.SpawnTerrainObject(group, proto, settings, rayHit, positionSpawn, centerPosition);
            }

            RandomUtility.ChangeRandomSeed();

            await UniTask.WaitUntil(() => SimulatedBodyStack.Count == 0);

            ScriptingSystem.RemoveColliders(rayHit);
        }
#endif
    }
}
#endif
