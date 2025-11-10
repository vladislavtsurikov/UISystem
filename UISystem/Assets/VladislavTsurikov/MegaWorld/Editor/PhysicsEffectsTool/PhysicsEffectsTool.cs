#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.MegaWorld.Editor.Common.Window;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Editor.PhysicsEffectsTool
{
    [Name("PhysX Painter/Physics Effects")]
    [SupportMultipleSelectedGroups]
    [SupportedPrototypeTypes(new[] { typeof(PrototypeGameObject) })]
    [AddGlobalCommonComponents(new[] { typeof(LayerSettings) })]
    [AddToolComponents(new[] { typeof(PhysicsEffectsToolSettings) })]
    [DontCreate]
    public class PhysicsEffectsTool : ToolWindow
    {
        private SpacingMouseMove _mouseMove = new();

        private PhysicsEffectsToolSettings _physicsEffectsToolSettings;

        protected override void OnEnable()
        {
            _physicsEffectsToolSettings =
                (PhysicsEffectsToolSettings)ToolsComponentStack.GetElement(typeof(PhysicsEffectsTool),
                    typeof(PhysicsEffectsToolSettings));

            _mouseMove = new SpacingMouseMove();
            _mouseMove.OnMouseDown += OnMouseDown;
            _mouseMove.OnMouseDrag += OnMouseDrag;
            _mouseMove.OnRepaint += OnRepaint;
        }

        protected override void DoTool()
        {
            _mouseMove.Spacing = _physicsEffectsToolSettings.Spacing;

            _mouseMove.Run();
        }

        private void OnMouseDown() => _physicsEffectsToolSettings.List.SelectedElement?.DoEffect(_mouseMove.Raycast);

        private void OnMouseDrag(Vector3 dragPoint)
        {
            RayHit rayHit = ColliderUtility.Raycast(RayUtility.GetRayDown(dragPoint),
                GlobalCommonComponentSingleton<LayerSettings>.Instance.PaintLayers);

            if (rayHit != null)
            {
                _physicsEffectsToolSettings.List.SelectedElement?.DoEffect(rayHit);
            }
        }

        private void OnRepaint() => _physicsEffectsToolSettings.List.SelectedElement?.OnRepaint(_mouseMove.Raycast);
    }
}
#endif
