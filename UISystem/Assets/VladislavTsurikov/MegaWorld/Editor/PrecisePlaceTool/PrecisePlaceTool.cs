#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Editor.Common.Window;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.PrototypeSettings;
using VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.Visualisation;
using VladislavTsurikov.MegaWorld.Runtime.Common;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Runtime;
using ToolsComponentStack = VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem.ToolsComponentStack;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool
{
    [Name("Happy Artist/Precise Place")]
    [SupportedPrototypeTypes(new[] { typeof(PrototypeTerrainObject), typeof(PrototypeGameObject) })]
    [AddGlobalCommonComponents(new[] { typeof(TransformSpaceSettings), typeof(LayerSettings) })]
    [AddToolComponents(new[] { typeof(PrecisePlaceToolSettings) })]
    [AddGeneralPrototypeComponents(new[] { typeof(PrototypeTerrainObject), typeof(PrototypeGameObject) },
        new[] { typeof(SuccessSettings), typeof(OverlapCheckSettings), typeof(TransformComponentSettings) })]
    [AddPrototypeComponents(new[] { typeof(PrototypeTerrainObject), typeof(PrototypeGameObject) },
        new[] { typeof(PrecisePlaceSettings), typeof(PrecisePlaceSettings) })]
    public class PrecisePlaceTool : ToolWindow
    {
        private SpacingMouseMove _mouseMove = new();

        private bool _mouseUp;

        private PrecisePlaceToolSettings _settings;

        protected override void OnEnable()
        {
            _settings = (PrecisePlaceToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePlaceTool),
                typeof(PrecisePlaceToolSettings));

            _mouseMove = new SpacingMouseMove();
            _mouseMove.OnMouseDown += OnMouseDown;
            _mouseMove.BeforePointDetectionFromMouseDrag += BeforePointDetectionFromMouseDrag;
            _mouseMove.OnMouseDrag += OnMouseDrag;
            _mouseMove.OnMouseMove += OnMouseMove;
            _mouseMove.OnRepaint += OnRepaint;
        }

        protected override void DoTool()
        {
            ActiveObjectController.DestroyObjectIfNecessary();

            float lookAtSize = 0;

            if (ActiveObjectController.PlacedObjectData != null)
            {
                Bounds bounds = ActiveObjectController.PlacedObjectData.GameObject.GetObjectWorldBounds();

                if (_mouseMove.Raycast != null)
                {
                    bounds.Encapsulate(_mouseMove.Raycast.Point);
                }

                if (bounds.size.magnitude > Mathf.Epsilon)
                {
                    lookAtSize = bounds.size.magnitude * 0.5f;
                }
                else
                {
                    lookAtSize = 0;
                }
            }

            _mouseMove.Spacing = _settings.Spacing;
            _mouseMove.LookAtSize = lookAtSize;

            GameObject ignoreGameObjectRaycast = null;

            if (ActiveObjectController.PlacedObjectData != null)
            {
                ignoreGameObjectRaycast = ActiveObjectController.PlacedObjectData.GameObject;
            }

            _mouseMove.IgnoreGameObjectRaycast = ignoreGameObjectRaycast;

            _mouseMove.Run();
        }

        protected override void OnDeselect()
        {
            if (ActiveObjectController.PlacedObjectData != null)
            {
                ActiveObjectController.DestroyObject();
            }
        }

        protected override void HandleKeyboardEvents()
        {
            var settings =
                (PrecisePlaceToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePlaceTool),
                    typeof(PrecisePlaceToolSettings));

            Tools.current = Tool.None;

            if (ActiveObjectController.PlacedObjectData == null)
            {
                return;
            }

            settings.MouseActionStack.CheckShortcutCombos(ActiveObjectController.PlacedObjectData.GameObject,
                _mouseMove.Raycast);

            if (PrecisePlaceToolAllShortcutCombos.Instance.Restore.IsActive())
            {
                ActiveObjectController.PlacedObjectData.GameObject.CopyTransform(
                    ActiveObjectController.PlacedObjectData.Proto.Prefab, false);
                ActiveObjectController.PlacedObjectData.Proto.PastTransform =
                    new PastTransform(ActiveObjectController.PlacedObjectData.GameObject.transform);
                Event.current.Use();
            }

            if (!settings.MouseActionStack.IsAnyMouseActionActive)
            {
                SelectionPrototypeUtility.ScrollWheelAction(_mouseMove);
            }
        }

        public override bool DisableToolIfUnityToolActive() => false;

        private void OnMouseDown()
        {
            Group group = WindowData.Instance.SelectionData.SelectedData.SelectedGroup;

            ActiveObjectController.PlacedObjectData = PlaceObjectUtility.TryToPlace(group,
                SelectionPrototypeUtility.GetSelectedProto(group), _mouseMove.Raycast);
        }

        private void BeforePointDetectionFromMouseDrag()
        {
            if (_settings.MouseActionStack.IsAnyMouseActionActive)
            {
                return;
            }

            if (ActiveObjectController.PlacedObjectData != null)
            {
                if (!PlaceObjectUtility.CanPlace(ActiveObjectController.PlacedObjectData.Proto,
                        ActiveObjectController.PlacedObjectData.GameObject))
                {
                    ActiveObjectController.DestroyObject();
                }
            }
        }

        private void OnMouseDrag(Vector3 dragPoint)
        {
            if (_settings.MouseActionStack.IsAnyMouseActionActive)
            {
                return;
            }

            Group group = WindowData.Instance.SelectionData.SelectedData.SelectedGroup;
            LayerSettings layerSettings = GlobalCommonComponentSingleton<LayerSettings>.Instance;

            RayHit rayHit = ColliderUtility.Raycast(RayUtility.GetRayDown(dragPoint),
                layerSettings.GetCurrentPaintLayers(group.PrototypeType));

            if (rayHit != null)
            {
                PlacedObjectPrototype proto = SelectionPrototypeUtility.GetSelectedProto(group);

                ActiveObjectController.PlacedObjectData =
                    PlaceObjectUtility.DragPlace(group, proto, rayHit, _mouseMove);
            }
        }

        private void OnMouseMove()
        {
            if (ActiveObjectController.PlacedObjectData == null)
            {
                Group group = WindowData.Instance.SelectionData.SelectedData.SelectedGroup;
                ActiveObjectController.PlacedObjectData = PlaceObjectUtility.TryToPlace(group,
                    SelectionPrototypeUtility.GetSelectedProto(group), _mouseMove.Raycast);
            }

            if (ActiveObjectController.PlacedObjectData != null)
            {
                _settings.MouseActionStack.OnMouseMove();
                ActiveObjectController.PlacedObjectData.Proto.PastTransform =
                    new PastTransform(ActiveObjectController.PlacedObjectData.GameObject.transform);

                ObjectActions.UpdateTransform(_mouseMove.Raycast);
            }
        }

        private void OnRepaint() => PrecisePlaceVisualisation.DrawVisualisation(_mouseMove);
    }
}
#endif
