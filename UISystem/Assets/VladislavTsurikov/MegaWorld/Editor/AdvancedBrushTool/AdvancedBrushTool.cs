#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Editor.Common.Window;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.BrushSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainDetail;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using VladislavTsurikov.ReflectionUtility;
using ToolsComponentStack = VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem.ToolsComponentStack;

namespace VladislavTsurikov.MegaWorld.Editor.AdvancedBrushTool
{
    [Name("Advanced Brush")]
    [SupportMultipleSelectedGroups]
    [SupportedPrototypeTypes(new[]
    {
        typeof(PrototypeTerrainObject), typeof(PrototypeGameObject), typeof(PrototypeTerrainDetail),
        typeof(PrototypeTerrainTexture)
    })]
    [AddGlobalCommonComponents(new[] { typeof(LayerSettings) })]
    [AddToolComponents(new[] { typeof(AdvancedBrushToolSettings), typeof(BrushSettings) })]
    [AddGeneralPrototypeComponents(new[] { typeof(PrototypeTerrainObject), typeof(PrototypeGameObject) },
        new[] { typeof(SuccessSettings), typeof(OverlapCheckSettings), typeof(TransformComponentSettings) })]
    [AddGeneralPrototypeComponents(typeof(PrototypeTerrainDetail),
        new[] { typeof(SpawnDetailSettings), typeof(MaskFilterComponentSettings) })]
    [AddGeneralPrototypeComponents(typeof(PrototypeTerrainTexture), new[] { typeof(MaskFilterComponentSettings) })]
    [AddGeneralGroupComponents(new[] { typeof(PrototypeTerrainObject), typeof(PrototypeGameObject) },
        new[] { typeof(ScatterComponentSettings), typeof(FilterSettings) })]
    public class AdvancedBrushTool : ToolWindow
    {
        private AdvancedBrushToolSettings _advancedBrushToolSettings;
        private BrushSettings _brushSettings;
        private SpacingMouseMove _mouseMove = new();

        protected override void OnEnable()
        {
            _advancedBrushToolSettings =
                (AdvancedBrushToolSettings)ToolsComponentStack.GetElement(typeof(AdvancedBrushTool),
                    typeof(AdvancedBrushToolSettings));
            _brushSettings =
                (BrushSettings)ToolsComponentStack.GetElement(typeof(AdvancedBrushTool), typeof(BrushSettings));

            _mouseMove = new SpacingMouseMove();
            _mouseMove.OnMouseDown += OnMouseDown;
            _mouseMove.OnMouseDrag += OnMouseDrag;
            _mouseMove.OnRepaint += OnRepaint;
        }

        protected override void DoTool()
        {
            var brushSpacing = _brushSettings.Spacing;
            if (_mouseMove.IsStartDrag)
            {
                if (WindowData.Instance.SelectedData.GetSelectedPrototypes(typeof(PrototypeTerrainObject)).Count != 0
                    || WindowData.Instance.SelectedData.GetSelectedPrototypes(typeof(PrototypeGameObject)).Count != 0)
                {
                    if (brushSpacing < _brushSettings.BrushSize / 2)
                    {
                        brushSpacing = _brushSettings.BrushSize / 2;
                    }
                }
            }

            _mouseMove.Spacing = brushSpacing;
            _mouseMove.LookAtSize = _brushSettings.BrushSize;

            _mouseMove.Run();
        }

        protected override void HandleKeyboardEvents()
        {
            var brushSettings =
                (BrushSettings)ToolsComponentStack.GetElement(typeof(AdvancedBrushTool), typeof(BrushSettings));

            brushSettings.ScrollBrushRadiusEvent();
        }

        private void OnMouseDown()
        {
            foreach (Group group in WindowData.Instance.SelectedData.SelectedGroupList)
            {
                BoxArea area = _brushSettings.BrushJitterSettings.GetAreaVariables(_brushSettings,
                    _mouseMove.Raycast.Point,
                    group);

                if (area.RayHit != null)
                {
                    PaintGroup(group, area);
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

                    if (area?.RayHit != null)
                    {
                        PaintGroup(group, area, _advancedBrushToolSettings.EnableFailureRateOnMouseDrag);
                    }
                }
            }
        }

        private void OnRepaint()
        {
            BoxArea area = _brushSettings.GetAreaVariables(_mouseMove.Raycast);
            AdvancedBrushToolVisualisation.Draw(area, WindowData.Instance.SelectionData,
                GlobalCommonComponentSingleton<LayerSettings>.Instance);
        }

        private void PaintGroup(Group group, BoxArea area, bool dragMouse = false)
        {
            var advancedBrushToolSettings =
                (AdvancedBrushToolSettings)ToolsComponentStack.GetElement(typeof(AdvancedBrushTool),
                    typeof(AdvancedBrushToolSettings));

            if (group.PrototypeType == typeof(PrototypeGameObject))
            {
                SpawnGroup.SpawnGroupGameObject(group, area, dragMouse).Forget();
            }
            else if (group.PrototypeType == typeof(PrototypeTerrainObject))
            {
                SpawnGroup.SpawnGroupTerrainObject(group, area, dragMouse).Forget();
            }
            else if (group.PrototypeType == typeof(PrototypeTerrainDetail))
            {
                Runtime.Common.Utility.Spawn.SpawnGroup.SpawnTerrainDetails(group, group.PrototypeList, area);
            }
            else if (group.PrototypeType == typeof(PrototypeTerrainTexture))
            {
                Runtime.Common.Utility.Spawn.SpawnGroup.SpawnTerrainTexture(group, group.GetAllSelectedPrototypes(),
                    area, advancedBrushToolSettings.TextureTargetStrength);
            }
        }
    }
}
#endif
