#if UNITY_EDITOR
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.EditorShortcutCombo.Editor;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
using VladislavTsurikov.Undo.Editor.GameObject;
using VladislavTsurikov.Undo.Editor.TerrainObjectRenderer;

namespace VladislavTsurikov.MegaWorld.Editor.EditTool.ActionSystem
{
    [Name("Raycast")]
    public class Raycast : Action
    {
        private ShortcutCombo _shortcutCombo;
        public LayerMask GroundLayers = 1;

        public float RaycastPositionOffset = 0;

        [OnDeserializing]
        private void OnDeserializing() => InitShortcutCombo();

        protected override void OnCreate() => InitShortcutCombo();

        private void InitShortcutCombo()
        {
            _shortcutCombo = new ShortcutCombo();
            _shortcutCombo.AddKey(KeyCode.W);
        }

        public override void OnMouseMove()
        {
            RayHit rayHit = ColliderUtility.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition),
                GroundLayers);
            if (rayHit != null)
            {
                var finalPosition = new Vector3(rayHit.Point.x, rayHit.Point.y + RaycastPositionOffset, rayHit.Point.z);
                EditTool.FindObject.Position = finalPosition;
            }
        }

        protected override void RegisterUndo()
        {
            if (EditTool.FindObject.PrototypeType == typeof(PrototypeGameObject))
            {
                var go = (GameObject)EditTool.FindObject.Obj;
                Undo.Editor.Undo.RegisterUndoAfterMouseUp(new GameObjectTransform(go));
            }
            else if (EditTool.FindObject.PrototypeType == typeof(PrototypeTerrainObject))
            {
                var instance = (TerrainObjectInstance)EditTool.FindObject.Obj;
                Undo.Editor.Undo.RegisterUndoAfterMouseUp(new TerrainObjectTransform(instance));
            }
        }

        public override bool CheckShortcutCombo()
        {
            if (_shortcutCombo.IsActive())
            {
                return true;
            }

            return false;
        }
    }
}
#endif
