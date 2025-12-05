#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Editor.Common.Window;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.BrushSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using VladislavTsurikov.ReflectionUtility;
using ToolsComponentStack = VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem.ToolsComponentStack;

namespace VladislavTsurikov.MegaWorld.Editor.BrushPhysicsTool
{
    [Name("PhysX Painter/Brush Physics")]
    [SupportMultipleSelectedGroups]
    [SupportedPrototypeTypes(new[] { typeof(PrototypeTerrainObject), typeof(PrototypeGameObject) })]
    [AddGlobalCommonComponents(new[] { typeof(LayerSettings) })]
    [AddToolComponents(new[] { typeof(BrushPhysicsToolSettings), typeof(PhysicsEffects), typeof(BrushSettings) })]
    [AddGeneralPrototypeComponents(new[] { typeof(PrototypeGameObject), typeof(PrototypeTerrainObject) },
        new[] { typeof(SuccessSettings), typeof(PhysicsTransformComponentSettings) })]
    [AddGeneralGroupComponents(new[] { typeof(PrototypeGameObject), typeof(PrototypeTerrainObject) },
        new[] { typeof(ScatterComponentSettings) })]
    public class BrushPhysicsTool : ToolWindow
    {
        private BrushSettings _brushSettings;
        private SpacingMouseMove _mouseMove = new();

        protected override void OnEnable()
        {
            _brushSettings =
                (BrushSettings)ToolsComponentStack.GetElement(typeof(BrushPhysicsTool), typeof(BrushSettings));

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

        protected override void HandleKeyboardEvents() => _brushSettings.ScrollBrushRadiusEvent();

        private void OnMouseDown()
        {
            foreach (Group group in WindowData.Instance.SelectedData.SelectedGroupList)
            {
                BoxArea area = _brushSettings.BrushJitterSettings.GetAreaVariables(_brushSettings,
                    _mouseMove.Raycast.Point,
                    group);

                if (area.RayHit != null)
                {
                    PaintGroup(group, area).Forget();
                }
            }
        }

        private void OnMouseDrag(Vector3 dragPoint)
        {
            foreach (Group group in WindowData.Instance.SelectedData.SelectedGroupList)
            {
                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(dragPoint),
                    GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(group.PrototypeType));

                if (rayHit != null)
                {
                    BoxArea area =
                        _brushSettings.BrushJitterSettings.GetAreaVariables(_brushSettings, rayHit.Point, group);

                    if (area.RayHit != null)
                    {
                        PaintGroup(group, area).Forget();
                    }
                }
            }
        }

        private void OnRepaint()
        {
            BoxArea area = _brushSettings.GetAreaVariables(_mouseMove.Raycast);
            BrushPhysicsToolVisualisation.Draw(area);
        }

        private async UniTask PaintGroup(Group group, BoxArea area)
        {
            if (group.PrototypeType == typeof(PrototypeGameObject))
            {
                await SpawnGroup.SpawnGameObject(group, area);
            }
#if RENDERER_STACK
            else if (group.PrototypeType == typeof(PrototypeTerrainObject))
            {
                await SpawnGroup.SpawnTerrainObject(group, area);
            }
#endif
        }
    }
}
#endif
