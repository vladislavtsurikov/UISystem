#if UNITY_EDITOR
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.EditorShortcutCombo.Editor;
using VladislavTsurikov.MegaWorld.Runtime.Common;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.MouseActions
{
    public enum MoveAlongAxis
    {
        SurfaceNormal,
        Y
    }

    [Name("Move Along Direction")]
    public class MoveAlongDirection : MouseAction
    {
        private ShortcutCombo _mainShortcutCombo;
        private Vector3 _startPosition;
        private float _surfaceOffset;
        public MouseSensitivitySettings MouseMoveAlongDirectionSettings = new();

        public MoveAlongAxis MoveAlongAxis = MoveAlongAxis.Y;
        public float WeightToNormal = 1;

        [OnDeserializing]
        private void OnDeserializing() => InitShortcutCombo();

        protected override void OnCreate() => InitShortcutCombo();

        private void InitShortcutCombo()
        {
            _mainShortcutCombo = new ShortcutCombo();
            _mainShortcutCombo.AddKey(KeyCode.Q);
        }

        public override void CheckShortcutCombos(GameObject gameObject, Vector3 normal)
        {
            if (_mainShortcutCombo.IsActive())
            {
                if (Begin(gameObject, normal))
                {
                    SetStartValue(gameObject);
                }
            }
            else
            {
                End();
            }
        }

        private void SetStartValue(GameObject gameObject)
        {
            _startPosition = gameObject.transform.position;
            _surfaceOffset = 0;
        }

        public override void OnMouseMove()
        {
            _surfaceOffset += Event.current.delta.x * MouseMoveAlongDirectionSettings.MouseSensitivity * 0.05f;

            Vector3 direction;

            if (MoveAlongAxis == MoveAlongAxis.SurfaceNormal)
            {
                direction = Vector3.Lerp(Vector3.up, Normal, WeightToNormal);
            }
            else
            {
                direction = Vector3.up;
            }

            GameObject.transform.position = _startPosition + direction * _surfaceOffset;
        }

        public override void OnRepaint()
        {
            Vector3 direction;

            if (MoveAlongAxis == MoveAlongAxis.SurfaceNormal)
            {
                direction = Vector3.Lerp(Vector3.up, Normal, WeightToNormal);
            }
            else
            {
                direction = Vector3.up;
            }

            Handles.color = Handles.selectedColor;
            Handles.ArrowHandleCap(0, GameObject.transform.position,
                Quaternion.FromToRotation(Vector3.forward, direction),
                0.5f * HandleUtility.GetHandleSize(GameObject.transform.position), EventType.Repaint);
        }
    }
}
#endif
