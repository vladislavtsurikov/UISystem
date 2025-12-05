#if UNITY_EDITOR
using System.Runtime.Serialization;
using UnityEngine;
using VladislavTsurikov.EditorShortcutCombo.Editor;
using VladislavTsurikov.MegaWorld.Runtime.Common;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
using VladislavTsurikov.Undo.Editor.GameObject;
using VladislavTsurikov.Undo.Editor.TerrainObjectRenderer;

namespace VladislavTsurikov.MegaWorld.Editor.EditTool.ActionSystem
{
    [Name("Scale")]
    public class Scale : Action
    {
        private float _scaleDist;
        private ShortcutCombo _shortcutCombo;

        private Vector3 _startScale;
        public bool EnableSnapScale = false;

        public MouseSensitivitySettings MouseSensitivitySettings = new();
        public float SnapScale = 1f;

        [OnDeserializing]
        private void OnDeserializing() => InitShortcutCombo();

        protected override void OnCreate() => InitShortcutCombo();

        private void InitShortcutCombo()
        {
            _shortcutCombo = new ShortcutCombo();
            _shortcutCombo.AddKey(KeyCode.R);
        }

        public override void OnMouseMove()
        {
            var mouseSensitivityScale = CalculateMouseSensitivityScale();
            var newMouseSensitivity = MouseSensitivitySettings.MouseSensitivity * mouseSensitivityScale * 0.03f;

            _scaleDist += Event.current.delta.x * newMouseSensitivity;

            UniformScaleObject(_scaleDist);
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

        public void UniformScaleObject(float scale)
        {
            if (EnableSnapScale)
            {
                EditTool.FindObject.Scale = Snapping.Snap(
                    new Vector3(_startScale.x + scale, _startScale.y + scale, _startScale.z + scale),
                    new Vector3(SnapScale, SnapScale, SnapScale));
            }
            else
            {
                EditTool.FindObject.Scale = (scale + 1) * _startScale;
            }
        }

        private float CalculateMouseSensitivityScale()
        {
            var maxAbsScaleComponent = Mathf.Abs(GetWithBiggestAbsValue());
            if (maxAbsScaleComponent < 1e-5f)
            {
                maxAbsScaleComponent = 0.001f;
            }

            return 1.0f / maxAbsScaleComponent;
        }

        public float GetWithBiggestAbsValue()
        {
            var maxComponent = _startScale.x;
            if (Mathf.Abs(maxComponent) < Mathf.Abs(_startScale.y))
            {
                maxComponent = _startScale.y;
            }

            if (Mathf.Abs(maxComponent) < Mathf.Abs(_startScale.z))
            {
                maxComponent = _startScale.z;
            }

            return maxComponent;
        }

        private void SetStartValue()
        {
            _startScale = EditTool.FindObject.Scale;
            _scaleDist = 0;
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
