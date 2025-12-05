#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.ColorUtility.Runtime;
using VladislavTsurikov.MegaWorld.Editor.Common.Window;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Editor;

namespace VladislavTsurikov.MegaWorld.Editor.ExplodePhysics
{
    [Name("PhysX Painter/Explode Physics")]
    [SupportMultipleSelectedGroups]
    [SupportedPrototypeTypes(new[] { typeof(PrototypeGameObject), typeof(PrototypeTerrainObject) })]
    [AddGlobalCommonComponents(new[] { typeof(LayerSettings) })]
    [AddToolComponents(new[] { typeof(ExplodePhysicsToolSettings) })]
    public class ExplodePhysicsTool : ToolWindow
    {
        private ExplodePhysicsToolSettings _explodePhysicsToolSettings;
        private SpacingMouseMove _mouseMove = new();

        protected override void OnEnable()
        {
            _explodePhysicsToolSettings = (ExplodePhysicsToolSettings)ToolsComponentStack.GetElement(
                typeof(ExplodePhysicsTool),
                typeof(ExplodePhysicsToolSettings));

            _mouseMove = new SpacingMouseMove();
            _mouseMove.OnMouseDown += OnMouseDown;
            _mouseMove.OnMouseDrag += OnMouseDrag;
            _mouseMove.OnRepaint += OnRepaint;
        }

        protected override void DoTool()
        {
            _mouseMove.Spacing = _explodePhysicsToolSettings.Spacing;
            _mouseMove.LookAtSize = _explodePhysicsToolSettings.Size;

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
                SpawnGroup.SpawnGameObject(group, rayHit);
            }
#if RENDERER_STACK
            else if (group.PrototypeType == typeof(PrototypeTerrainObject))
            {
                await SpawnGroup.SpawnTerrainObject(group, rayHit);
            }
#endif
        }

        private void OnRepaint()
        {
            Color color = new Color(0.2f, 0.5f, 0.7f).WithAlpha(0.8f);
            Vector3 positionUp = _mouseMove.Raycast.Point +
                                 new Vector3(0, _explodePhysicsToolSettings.PositionOffsetY, 0);

            if (_explodePhysicsToolSettings.SpawnFromOnePoint)
            {
                DrawHandles.HandleButton(0, positionUp, color, 0.7f);
            }
            else
            {
                Handles.color = color;
                Handles.SphereHandleCap(0, positionUp, Quaternion.identity, _explodePhysicsToolSettings.Size,
                    EventType.Repaint);
                Handles.color = color.WithAlpha(1);
                Handles.DrawDottedLine(_mouseMove.Raycast.Point, positionUp, 2f);
            }
        }
    }
}
#endif
