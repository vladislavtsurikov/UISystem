#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter;
using VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter.Undo;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.PhysicsSimulator.Runtime;
using VladislavTsurikov.Undo.Editor.GameObject;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.ExplodePhysics
{
    public static class SpawnPrototype
    {
#if RENDERER_STACK
        public static void SpawnTerrainObject(Group group, PrototypeTerrainObject proto,
            ExplodePhysicsToolSettings settings, RayHit rayHit, Vector3 positionSpawn, Vector3 centerPosition)
        {
            var instance =
                new Instance(positionSpawn, proto.Prefab.transform.lossyScale, proto.Prefab.transform.rotation);

            var transformComponentSettings =
                (PhysicsTransformComponentSettings)proto.GetElement(typeof(PhysicsTransformComponentSettings));
            transformComponentSettings.TransformComponentStack.ManipulateTransform(ref instance, 1, rayHit.Normal);

            var onDisableSimulatedBodyAction =
                new TerrainObjectOnDisablePhysics(group, proto.RendererPrototype);

            PhysicsSimulator.Runtime.PhysicsSimulator.UseAccelerationPhysics = true;
            PhysicsSimulator.Runtime.PhysicsSimulator.SetDisablePhysicsMode<ObjectTimeDisablePhysicsMode>();

            TerrainObjectSimulatedBody simulatedBody =
                SimulatedBodyStack.InstantiateSimulatedBody<TerrainObjectSimulatedBody>(proto.Prefab,
                    instance.Position, instance.Scale, instance.Rotation,
                    new List<OnDisableSimulatedBodyEvent> { onDisableSimulatedBodyAction });

            group.GetDefaultElement<ContainerForGameObjects>().ParentGameObject(simulatedBody.GameObject);

            if (!settings.SpawnFromOnePoint)
            {
                Vector3 normal = (instance.Position - centerPosition).normalized;
                PhysicsUtility.ApplyForce(simulatedBody.Rigidbody, normal * settings.Force);
            }
            else
            {
                PhysicsUtility.ApplyForce(simulatedBody.Rigidbody, Random.insideUnitSphere.normalized);
            }

            Undo.Editor.Undo.RegisterUndoAfterMouseUp(new CreatedTerrainObjectSimulatedBody(simulatedBody));
        }
#endif

        public static void SpawnGameObject(Group group, PrototypeGameObject proto, ExplodePhysicsToolSettings settings,
            RayHit rayHit, Vector3 positionSpawn, Vector3 centerPosition)
        {
            var instance = new Instance(positionSpawn, proto.Prefab.transform.lossyScale,
                proto.Prefab.transform.rotation);

            var transformComponentSettings =
                (PhysicsTransformComponentSettings)proto.GetElement(typeof(PhysicsTransformComponentSettings));
            transformComponentSettings.TransformComponentStack.ManipulateTransform(ref instance, 1, rayHit.Normal);

            PhysicsSimulator.Runtime.PhysicsSimulator.UseAccelerationPhysics = true;
            PhysicsSimulator.Runtime.PhysicsSimulator.SetDisablePhysicsMode<ObjectTimeDisablePhysicsMode>();

            SimulatedBody simulatedBody = SimulatedBodyStack.InstantiateSimulatedBody(proto.Prefab,
                instance.Position, instance.Scale, instance.Rotation);

            group.GetDefaultElement<ContainerForGameObjects>().ParentGameObject(simulatedBody.GameObject);

            if (!settings.SpawnFromOnePoint)
            {
                Vector3 normal = (instance.Position - centerPosition).normalized;
                PhysicsUtility.ApplyForce(simulatedBody.Rigidbody, normal * settings.Force);
            }
            else
            {
                PhysicsUtility.ApplyForce(simulatedBody.Rigidbody, Random.insideUnitSphere.normalized);
            }

            Undo.Editor.Undo.RegisterUndoAfterMouseUp(new CreatedGameObject(simulatedBody.GameObject));
        }
    }
}
#endif
