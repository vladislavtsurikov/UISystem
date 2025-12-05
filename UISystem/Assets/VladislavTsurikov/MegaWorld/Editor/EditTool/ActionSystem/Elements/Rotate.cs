#if UNITY_EDITOR
using System.Runtime.Serialization;
using UnityEngine;
using VladislavTsurikov.EditorShortcutCombo.Editor;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
using VladislavTsurikov.Undo.Editor.GameObject;
using VladislavTsurikov.Undo.Editor.TerrainObjectRenderer;

namespace VladislavTsurikov.MegaWorld.Editor.EditTool.ActionSystem
{
    [Name("Rotate")]
    public class Rotate : Action
    {
        private Quaternion _сurrentRotation;
        private Vector3 _rotationAxis;

        private float _rotationDist;
        private ShortcutCombo _shortcutCombo;
        private Quaternion _startRotation;
        public bool EnableSnapRotate = false;

        public MouseSensitivitySettings MouseSensitivitySettings = new();
        public float SnapRotate = 15f;

        [OnDeserializing]
        private void OnDeserializing() => InitShortcutCombo();

        protected override void OnCreate() => InitShortcutCombo();

        private void InitShortcutCombo()
        {
            _shortcutCombo = new ShortcutCombo();
            _shortcutCombo.AddKey(KeyCode.E);
        }

        public override void OnMouseMove()
        {
            _rotationDist += Event.current.delta.x * MouseSensitivitySettings.MouseSensitivity * 2f;
            var angle = _rotationDist;
            if (EnableSnapRotate)
            {
                angle = Snapping.Snap(_rotationDist, SnapRotate);
            }

            _сurrentRotation = Quaternion.AngleAxis(angle, _rotationAxis);
            EditTool.FindObject.Rotation = _сurrentRotation * _startRotation;
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

        protected override void OnObjectFound() => SetStartValue();

        public override bool CheckShortcutCombo()
        {
            if (_shortcutCombo.IsActive())
            {
                return true;
            }

            return false;
        }

        private void SetStartValue()
        {
            _rotationAxis = Vector3.up;
            if (GlobalCommonComponentSingleton<TransformSpaceSettings>.Instance.TransformSpace == TransformSpace.Local)
            {
                _rotationAxis = EditTool.FindObject.Rotation * _rotationAxis;
            }

            _rotationDist = 0;
            _startRotation = EditTool.FindObject.Rotation;
            _сurrentRotation = Quaternion.identity;
        }
    }
}
#endif
