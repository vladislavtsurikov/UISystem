#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.ColorUtility.Runtime;
using VladislavTsurikov.MegaWorld.Editor.Common.Window;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Editor;
using VladislavTsurikov.UnityUtility.Runtime;
using ToolsComponentStack = VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem.ToolsComponentStack;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePhysicsTool
{
    [Name("PhysX Painter/Precise Physics")]
    [SupportMultipleSelectedGroups]
    [SupportedPrototypeTypes(new[] { typeof(PrototypeTerrainObject), typeof(PrototypeGameObject) })]
    [AddGlobalCommonComponents(new[] { typeof(LayerSettings) })]
    [AddToolComponents(new[] { typeof(PrecisePhysicsToolSettings), typeof(PhysicsEffects) })]
    [AddGeneralPrototypeComponents(new[] { typeof(PrototypeGameObject), typeof(PrototypeTerrainObject) },
        new[] { typeof(SuccessSettings), typeof(PhysicsTransformComponentSettings) })]
    public class PrecisePhysicsTool : ToolWindow
    {
        private SpacingMouseMove _mouseMove = new();

        private PrecisePhysicsToolSettings _precisePhysicsToolSettings;

        protected override void OnEnable()
        {
            _precisePhysicsToolSettings =
                (PrecisePhysicsToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePhysicsTool),
                    typeof(PrecisePhysicsToolSettings));

            _mouseMove = new SpacingMouseMove();
            _mouseMove.OnMouseDown += OnMouseDown;
            _mouseMove.OnMouseDrag += OnMouseDrag;
            _mouseMove.OnRepaint += OnRepaint;
        }

        protected override void DoTool()
        {
            _mouseMove.Spacing = _precisePhysicsToolSettings.Spacing;

            _mouseMove.Run();
        }

        private void OnMouseDown()
        {
            Group group = WindowData.Instance.SelectionData.SelectedData.SelectedGroup;

            PaintGroup(group, _mouseMove.Raycast).Forget();
        }

        private void OnMouseDrag(Vector3 dragPoint)
        {
            Group group = WindowData.Instance.SelectionData.SelectedData.SelectedGroup;
            LayerSettings layerSettings = GlobalCommonComponentSingleton<LayerSettings>.Instance;

            RayHit rayHit = ColliderUtility.Raycast(RayUtility.GetRayDown(dragPoint),
                layerSettings.GetCurrentPaintLayers(group.PrototypeType));

            if (rayHit != null)
            {
                PaintGroup(group, rayHit).Forget();
            }
        }

        private async UniTask PaintGroup(Group group, RayHit rayHit)
        {
            if (group.PrototypeType == typeof(PrototypeGameObject))
            {
                var proto =
                    (PrototypeGameObject)GetRandomPrototype.GetMaxSuccessProto(group.GetAllSelectedPrototypes());
                SpawnPrototype.SpawnGameObject(group, proto, rayHit);
            }
#if RENDERER_STACK
            else if (group.PrototypeType == typeof(PrototypeTerrainObject))
            {
                var proto =
                    (PrototypeTerrainObject)GetRandomPrototype.GetMaxSuccessProto(group.GetAllSelectedPrototypes());
                await SpawnPrototype.SpawnTerrainObject(group, proto, rayHit);
            }
#endif

            RandomUtility.ChangeRandomSeed();
        }

        private void OnRepaint()
        {
            Color color = new Color(0.2f, 0.5f, 0.7f).WithAlpha(0.8f);
            Vector3 position = _mouseMove.Raycast.Point +
                               new Vector3(0, _precisePhysicsToolSettings.PositionOffsetY, 0);
            DrawHandles.HandleButton(0, position, color, 0.7f);
        }
    }
}
#endif
