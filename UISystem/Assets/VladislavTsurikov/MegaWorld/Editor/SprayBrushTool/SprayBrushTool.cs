#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Editor.Common.Window;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
using VladislavTsurikov.Undo.Editor.GameObject;
using VladislavTsurikov.Undo.Editor.TerrainObjectRenderer;
using GameObjectUtility = VladislavTsurikov.MegaWorld.Runtime.Core.Utility.GameObjectUtility;
using Instance = VladislavTsurikov.UnityUtility.Runtime.Instance;
using PrototypeTerrainObject =
    VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject.
    PrototypeTerrainObject;
using ToolsComponentStack = VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem.ToolsComponentStack;

namespace VladislavTsurikov.MegaWorld.Editor.SprayBrushTool
{
    [Name("Spray Brush")]
    [SupportMultipleSelectedGroups]
    [SupportedPrototypeTypes(new[] { typeof(PrototypeTerrainObject), typeof(PrototypeGameObject) })]
    [AddGlobalCommonComponents(new[] { typeof(LayerSettings) })]
    [AddGeneralPrototypeComponents(typeof(PrototypeTerrainObject),
        new[] { typeof(SuccessSettings), typeof(OverlapCheckSettings) })]
    [AddGeneralPrototypeComponents(typeof(PrototypeGameObject),
        new[] { typeof(SuccessSettings), typeof(OverlapCheckSettings) })]
    [AddPrototypeComponents(typeof(PrototypeTerrainObject), new[] { typeof(SimpleTransformComponentSettings) })]
    [AddPrototypeComponents(typeof(PrototypeGameObject), new[] { typeof(SimpleTransformComponentSettings) })]
    [AddGroupComponents(new[] { typeof(PrototypeTerrainObject), typeof(PrototypeGameObject) },
        new[] { typeof(SimpleFilter) })]
    [AddToolComponents(new[] { typeof(BrushSettings) })]
    public class SprayBrushTool : ToolWindow
    {
        private BrushSettings _brushSettings;
        private SpacingMouseMove _mouseMove = new();

        protected override void OnEnable()
        {
            _brushSettings =
                (BrushSettings)ToolsComponentStack.GetElement(typeof(SprayBrushTool), typeof(BrushSettings));

            _mouseMove = new SpacingMouseMove();
            _mouseMove.OnMouseDown += OnMouseDown;
            _mouseMove.OnMouseDrag += OnMouseDrag;
            _mouseMove.OnRepaint += OnRepaint;
        }

        protected override void DoTool()
        {
            _mouseMove.Spacing = _brushSettings.Spacing;
            _mouseMove.LookAtSize = _brushSettings.BrushSize;

            _mouseMove.Run();
        }

        protected override void HandleKeyboardEvents()
        {
            var brushSettings =
                (BrushSettings)ToolsComponentStack.GetElement(typeof(SprayBrushTool), typeof(BrushSettings));

            brushSettings.ScrollBrushRadiusEvent();
        }

        private void OnMouseDown()
        {
            foreach (Group group in WindowData.Instance.SelectedData.SelectedGroupList)
            {
                BoxArea boxArea = _brushSettings.GetAreaVariables(_mouseMove.Raycast);

                if (boxArea.RayHit != null)
                {
                    PaintGroup(group, boxArea);
                }
            }
        }

        private void OnMouseDrag(Vector3 dragPoint)
        {
            foreach (Group group in WindowData.Instance.SelectedData.SelectedGroupList)
            {
                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayFromCameraPosition(dragPoint),
                    GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(group.PrototypeType));

                if (rayHit != null)
                {
                    BoxArea boxArea = _brushSettings.GetAreaVariables(rayHit);

                    if (boxArea.RayHit != null)
                    {
                        PaintGroup(group, boxArea);
                    }
                }
            }
        }

        private void OnRepaint()
        {
            BoxArea boxArea = _brushSettings.GetAreaVariables(_mouseMove.Raycast);
            SprayBrushToolVisualisation.Draw(boxArea);
        }

        private void PaintGroup(Group group, BoxArea boxArea)
        {
            if (group.PrototypeType == typeof(PrototypeGameObject))
            {
                SpawnGroupGameObject(group, boxArea);
            }
            else if (group.PrototypeType == typeof(PrototypeTerrainObject))
            {
                SpawnGroupTerrainObject(group, boxArea);
            }
        }

        private void SpawnGroupTerrainObject(Group group, BoxArea boxArea)
        {
            Vector3 positionForSpawn = boxArea.RayHit.Point +
                                       Vector3.ProjectOnPlane(Random.onUnitSphere, boxArea.RayHit.Normal) *
                                       boxArea.Radius;

            RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayFromCameraPosition(positionForSpawn),
                GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(group.PrototypeType));
            if (rayHit != null)
            {
                var proto =
                    (PrototypeTerrainObject)GetRandomPrototype.GetMaxSuccessProto(group.GetAllSelectedPrototypes());

                if (proto == null || proto.Active == false)
                {
                    return;
                }

                var fitness =
                    GetFitness.GetFromSimpleFilter((SimpleFilter)group.GetElement(GetType(), typeof(SimpleFilter)),
                        rayHit);

                if (fitness != 0)
                {
                    SpawnTerrainObject(proto, rayHit, fitness);
                }
            }
        }

        private void SpawnGroupGameObject(Group group, BoxArea boxArea)
        {
            Vector3 positionForSpawn = boxArea.RayHit.Point +
                                       Vector3.ProjectOnPlane(Random.onUnitSphere, boxArea.RayHit.Normal) *
                                       boxArea.Radius;

            RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayFromCameraPosition(positionForSpawn),
                GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(group.PrototypeType));
            if (rayHit != null)
            {
                var proto =
                    (PrototypeGameObject)GetRandomPrototype.GetMaxSuccessProto(group.GetAllSelectedPrototypes());

                if (proto == null || proto.Active == false)
                {
                    return;
                }

                var fitness =
                    GetFitness.GetFromSimpleFilter((SimpleFilter)group.GetElement(GetType(), typeof(SimpleFilter)),
                        rayHit);

                if (fitness != 0)
                {
                    SpawnGameObject(group, proto, rayHit, fitness);
                }
            }
        }

        private static void SpawnTerrainObject(PrototypeTerrainObject proto, RayHit rayHit, float fitness)
        {
#if RENDERER_STACK
            var overlapCheckSettings =
                (OverlapCheckSettings)proto.GetElement(typeof(OverlapCheckSettings));

            var instance =
                new Instance(rayHit.Point, proto.Prefab.transform.lossyScale, proto.Prefab.transform.rotation);

            var transformComponentSettings =
                (SimpleTransformComponentSettings)proto.GetElement(typeof(SprayBrushTool),
                    typeof(SimpleTransformComponentSettings));
            transformComponentSettings.TransformComponentStack.ManipulateTransform(ref instance, fitness,
                rayHit.Normal);

            if (OverlapCheckSettings.RunOverlapCheck(proto.GetType(), overlapCheckSettings, proto.Extents, instance))
            {
                TerrainObjectInstance terrainObjectInstance =
                    TerrainObjectRendererAPI.AddInstance(proto.RendererPrototype, instance.Position, instance.Scale,
                        instance.Rotation);
                Undo.Editor.Undo.RegisterUndoAfterMouseUp(new CreatedTerrainObject(terrainObjectInstance));
            }
#endif
        }

        private static void SpawnGameObject(Group group, PrototypeGameObject proto, RayHit rayHit, float fitness)
        {
            var overlapCheckSettings = (OverlapCheckSettings)proto.GetElement(typeof(OverlapCheckSettings));

            var instance = new Instance(rayHit.Point, proto.Prefab.transform.lossyScale,
                proto.Prefab.transform.rotation);

            var transformComponentSettings =
                (SimpleTransformComponentSettings)proto.GetElement(typeof(SprayBrushTool),
                    typeof(SimpleTransformComponentSettings));
            transformComponentSettings.TransformComponentStack.ManipulateTransform(ref instance, fitness,
                rayHit.Normal);

            if (OverlapCheckSettings.RunOverlapCheck(proto.GetType(), overlapCheckSettings, proto.Extents, instance))
            {
                GameObject gameObject = GameObjectUtility.Instantiate(proto.Prefab, instance.Position, instance.Scale,
                    instance.Rotation);
                group.GetDefaultElement<ContainerForGameObjects>().ParentGameObject(gameObject);

                GameObjectCollider.Editor.GameObjectCollider.RegisterGameObjectToCurrentScene(gameObject);

                Undo.Editor.Undo.RegisterUndoAfterMouseUp(new CreatedGameObject(gameObject));

                gameObject.transform.hasChanged = false;
            }
        }
    }
}
#endif
