#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter;
using VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter.Undo;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.PhysicsSimulator.Runtime;
using VladislavTsurikov.Undo.Editor.GameObject;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.BrushPhysicsTool
{
    public static class SpawnPrototype
    {
#if RENDERER_STACK
        public static void SpawnTerrainObject(Group group, PrototypeTerrainObject proto, BoxArea boxArea, RayHit rayHit)
        {
            var fitness = TextureUtility.GetFromWorldPosition(boxArea.Bounds, rayHit.Point, boxArea.Mask);

            if (fitness == 0)
            {
                return;
            }

            if (Random.Range(0f, 1f) < 1 - fitness)
            {
                return;
            }

            var brushPhysicsToolSettings =
                (BrushPhysicsToolSettings)ToolsComponentStack.GetElement(typeof(BrushPhysicsTool),
                    typeof(BrushPhysicsToolSettings));

            var instance =
                new Instance(rayHit.Point + new Vector3(0, brushPhysicsToolSettings.PositionOffsetY, 0),
                    proto.Prefab.transform.lossyScale, proto.Prefab.transform.rotation);

            var transformComponentSettings =
                (PhysicsTransformComponentSettings)proto.GetElement(typeof(PhysicsTransformComponentSettings));
            transformComponentSettings.TransformComponentStack.ManipulateTransform(ref instance, fitness,
                rayHit.Normal);

            var onDisableSimulatedBodyAction =
                new TerrainObjectOnDisablePhysics(group, proto.RendererPrototype);

            PhysicsSimulator.Runtime.PhysicsSimulator.UseAccelerationPhysics = true;
            PhysicsSimulator.Runtime.PhysicsSimulator.SetDisablePhysicsMode<ObjectTimeDisablePhysicsMode>();

            TerrainObjectSimulatedBody simulatedBody =
                SimulatedBodyStack.InstantiateSimulatedBody<TerrainObjectSimulatedBody>(proto.Prefab,
                    instance.Position, instance.Scale, instance.Rotation,
                    new List<OnDisableSimulatedBodyEvent> { onDisableSimulatedBodyAction });

            group.GetDefaultElement<ContainerForGameObjects>().ParentGameObject(simulatedBody.GameObject);

            brushPhysicsToolSettings.PhysicsEffects.ApplyForce(simulatedBody.Rigidbody);

            Undo.Editor.Undo.RegisterUndoAfterMouseUp(new CreatedTerrainObjectSimulatedBody(simulatedBody));
        }
#endif

        public static void SpawnGameObject(Group group, PrototypeGameObject proto, BoxArea boxArea, RayHit rayHit)
        {
            var fitness = TextureUtility.GetFromWorldPosition(boxArea.Bounds, rayHit.Point, boxArea.Mask);

            if (fitness == 0)
            {
                return;
            }

            if (Random.Range(0f, 1f) < 1 - fitness)
            {
                return;
            }

            var brushPhysicsToolSettings =
                (BrushPhysicsToolSettings)ToolsComponentStack.GetElement(typeof(BrushPhysicsTool),
                    typeof(BrushPhysicsToolSettings));

            var instance = new Instance(rayHit.Point + new Vector3(0, brushPhysicsToolSettings.PositionOffsetY, 0),
                proto.Prefab.transform.lossyScale, proto.Prefab.transform.rotation);

            var transformComponentSettings =
                (PhysicsTransformComponentSettings)proto.GetElement(typeof(PhysicsTransformComponentSettings));
            transformComponentSettings.TransformComponentStack.ManipulateTransform(ref instance, fitness,
                rayHit.Normal);

            PhysicsSimulator.Runtime.PhysicsSimulator.UseAccelerationPhysics = true;
            PhysicsSimulator.Runtime.PhysicsSimulator.SetDisablePhysicsMode<ObjectTimeDisablePhysicsMode>();

            SimulatedBody simulatedBody = SimulatedBodyStack.InstantiateSimulatedBody(proto.Prefab,
                instance.Position, instance.Scale, instance.Rotation);

            group.GetDefaultElement<ContainerForGameObjects>().ParentGameObject(simulatedBody.GameObject);

            brushPhysicsToolSettings.PhysicsEffects.ApplyForce(simulatedBody.Rigidbody);

            Undo.Editor.Undo.RegisterUndoAfterMouseUp(new CreatedGameObject(simulatedBody.GameObject));
        }
    }
}
#endif
