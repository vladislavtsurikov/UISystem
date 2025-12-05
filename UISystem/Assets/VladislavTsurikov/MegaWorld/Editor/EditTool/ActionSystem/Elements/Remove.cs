#if UNITY_EDITOR
using System.Runtime.Serialization;
using UnityEngine;
using VladislavTsurikov.EditorShortcutCombo.Editor;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
using VladislavTsurikov.Undo.Editor.GameObject;
using VladislavTsurikov.Undo.Editor.TerrainObjectRenderer;

namespace VladislavTsurikov.MegaWorld.Editor.EditTool.ActionSystem
{
    [Name("Remove")]
    public class Remove : Action
    {
        private ShortcutCombo _shortcutCombo;

        [OnDeserializing]
        private void OnDeserializing() => InitShortcutCombo();

        protected override void OnCreate() => InitShortcutCombo();

        private void InitShortcutCombo()
        {
            _shortcutCombo = new ShortcutCombo();
            _shortcutCombo.AddKey(KeyCode.T);
        }

        protected override void OnObjectFound()
        {
            EditTool.FindObject.DestroyObject();
            EditTool.FindObject = null;
        }

        protected override void RegisterUndo()
        {
            if (EditTool.FindObject.PrototypeType == typeof(PrototypeGameObject))
            {
                var go = (GameObject)EditTool.FindObject.Obj;
                Undo.Editor.Undo.RegisterUndoAfterMouseUp(new DestroyedGameObject(go));
            }
            else if (EditTool.FindObject.PrototypeType == typeof(PrototypeTerrainObject))
            {
                var instance = (TerrainObjectInstance)EditTool.FindObject.Obj;
                Undo.Editor.Undo.RegisterUndoAfterMouseUp(new DestroyedTerrainObject(instance));
            }
        }

        protected override Color GetColorHandleButton() => new(1f, 0f, 0f, 0.7f);

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
