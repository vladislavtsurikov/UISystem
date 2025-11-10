using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter;
using VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.PhysicsSimulator.Runtime;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.GravitySpawner.Utility
{
    public static class SpawnPrototype
    {
        public static void SpawnGameObject(GravitySpawner gravitySpawner, Group group, PrototypeGameObject proto,
            TerrainsMaskManager terrainsMaskManager, BoxArea boxArea, RayHit rayHit)
        {
            var fitness = TextureUtility.GetFromWorldPosition(boxArea.Bounds, rayHit.Point, boxArea.Mask);

            if (fitness != 0)
            {
                if (Random.Range(0f, 1f) < 1 - fitness)
                {
                    return;
                }

                var instance = new Instance(rayHit.Point + new Vector3(0, 30, 0), proto.Prefab.transform.lossyScale,
                    proto.Prefab.transform.rotation);

                var transformComponentSettings =
                    (PhysicsTransformComponentSettings)proto.GetElement(typeof(PhysicsTransformComponentSettings));
                transformComponentSettings.TransformComponentStack.ManipulateTransform(ref instance, fitness,
                    rayHit.Normal);

                var gravitySpawnerGameObject = new GravitySpawnerGameObject(group, terrainsMaskManager, boxArea);

                PhysicsSimulator.Runtime.PhysicsSimulator.UseAccelerationPhysics = true;
                PhysicsSimulator.Runtime.PhysicsSimulator.SetDisablePhysicsMode<ObjectTimeDisablePhysicsMode>();

                SimulatedBody simulatedBody = SimulatedBodyStack.InstantiateSimulatedBody(proto.Prefab,
                    instance.Position, instance.Scale, instance.Rotation,
                    new List<OnDisableSimulatedBodyEvent> { gravitySpawnerGameObject });

                group.GetDefaultElement<ContainerForGameObjects>().ParentGameObject(simulatedBody.GameObject);

                gravitySpawner.PhysicsEffects.ApplyForce(simulatedBody.Rigidbody);

                RandomUtility.ChangeRandomSeed();
            }
        }

#if RENDERER_STACK
        public static void SpawnTerrainObject(GravitySpawner gravitySpawner, Group group, PrototypeTerrainObject proto,
            TerrainsMaskManager terrainsMaskManager, BoxArea boxArea, RayHit rayHit)
        {
            var fitness = TextureUtility.GetFromWorldPosition(boxArea.Bounds, rayHit.Point, boxArea.Mask);

            if (fitness != 0)
            {
                if (Random.Range(0f, 1f) < 1 - fitness)
                {
                    return;
                }

                var instance =
                    new Instance(rayHit.Point + new Vector3(0, 30, 0), proto.Prefab.transform.lossyScale,
                        proto.Prefab.transform.rotation);

                var transformComponentSettings =
                    (PhysicsTransformComponentSettings)proto.GetElement(typeof(PhysicsTransformComponentSettings));
                transformComponentSettings.TransformComponentStack.ManipulateTransform(ref instance, fitness,
                    rayHit.Normal);

                var gravitySpawnerTerrainObject =
                    new GravitySpawnerTerrainObject(group, proto.RendererPrototype, terrainsMaskManager, boxArea);

                PhysicsSimulator.Runtime.PhysicsSimulator.UseAccelerationPhysics = true;
                PhysicsSimulator.Runtime.PhysicsSimulator.SetDisablePhysicsMode<ObjectTimeDisablePhysicsMode>();

                TerrainObjectSimulatedBody simulatedBody =
                    SimulatedBodyStack.InstantiateSimulatedBody<TerrainObjectSimulatedBody>(proto.Prefab,
                        instance.Position, instance.Scale, instance.Rotation,
                        new List<OnDisableSimulatedBodyEvent> { gravitySpawnerTerrainObject });

                group.GetDefaultElement<ContainerForGameObjects>().ParentGameObject(simulatedBody.GameObject);

                gravitySpawner.PhysicsEffects.ApplyForce(simulatedBody.Rigidbody);

                RandomUtility.ChangeRandomSeed();
            }
        }
#endif
    }
}
