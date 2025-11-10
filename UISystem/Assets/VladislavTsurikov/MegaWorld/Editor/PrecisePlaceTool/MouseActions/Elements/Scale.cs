#if UNITY_EDITOR
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.EditorShortcutCombo.Editor;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.MouseActions
{
    public enum ScaleMode
    {
        ScaleAxisX,
        ScaleAxisY,
        ScaleAxisZ,
        UniformScale
    }

    [Name("Scale")]
    public class Scale : MouseAction
    {
        private ShortcutCombo _mouseScaleX;
        private ShortcutCombo _mouseScaleY;
        private ShortcutCombo _mouseScaleZ;

        private ShortcutCombo _mouseUniformScale;
        private float _scaleDist;
        private ScaleMode _scaleMode;
        private Vector3 _startScale;
        public bool EnableSnapScale = false;

        public MouseSensitivitySettings MouseScaleSettings = new();
        public float SnapScale = 1f;

        [OnDeserializing]
        private void OnDeserializing() => InitShortcutCombo();

        protected override void OnCreate() => InitShortcutCombo();

        private void InitShortcutCombo()
        {
            _mouseUniformScale = new ShortcutCombo();
            _mouseUniformScale.AddKey(KeyCode.LeftShift);
            _mouseUniformScale.AddKey(KeyCode.LeftControl);

            _mouseScaleX = new ShortcutCombo();
            _mouseScaleX.AddKey(KeyCode.X);
            _mouseScaleX.AddShortcutCombo(_mouseUniformScale);

            _mouseScaleY = new ShortcutCombo();
            _mouseScaleY.AddKey(KeyCode.Y);
            _mouseScaleY.AddShortcutCombo(_mouseUniformScale);

            _mouseScaleZ = new ShortcutCombo();
            _mouseScaleZ.AddKey(KeyCode.Z);
            _mouseScaleZ.AddShortcutCombo(_mouseUniformScale);
        }

        public override void CheckShortcutCombos(GameObject gameObject, Vector3 normal)
        {
            if (_mouseUniformScale.IsActive())
            {
                if (Begin(gameObject, normal) || _scaleMode != ScaleMode.UniformScale)
                {
                    SetStartValue(gameObject);
                }

                _scaleMode = ScaleMode.UniformScale;
            }
            else if (_mouseScaleX.IsActive())
            {
                if (Begin(gameObject, normal) || _scaleMode != ScaleMode.ScaleAxisX)
                {
                    SetStartValue(gameObject);
                }

                _scaleMode = ScaleMode.ScaleAxisX;
            }
            else if (_mouseScaleY.IsActive())
            {
                if (Begin(gameObject, normal) || _scaleMode != ScaleMode.ScaleAxisY)
                {
                    SetStartValue(gameObject);
                }

                _scaleMode = ScaleMode.ScaleAxisY;
            }
            else if (_mouseScaleZ.IsActive())
            {
                if (Begin(gameObject, normal) || _scaleMode != ScaleMode.ScaleAxisZ)
                {
                    SetStartValue(gameObject);
                }

                _scaleMode = ScaleMode.ScaleAxisZ;
            }
            else
            {
                End();
            }
        }

        public void SetStartValue(GameObject gameObject)
        {
            _startScale = gameObject.transform.lossyScale;
            _scaleDist = 0;
            GameObject = gameObject;
        }

        public override void OnMouseMove()
        {
            if (_active && GameObject != null)
            {
                var mouseSensitivityScale = CalculateMouseSensitivityScale();
                var newMouseSensitivity = MouseScaleSettings.MouseSensitivity * mouseSensitivityScale * 0.03f;

                _scaleDist += Event.current.delta.x * newMouseSensitivity;

                switch (_scaleMode)
                {
                    case ScaleMode.ScaleAxisX:
                    {
                        ScaleObjectAxis(Axis.X, _scaleDist);
                        break;
                    }
                    case ScaleMode.ScaleAxisY:
                    {
                        ScaleObjectAxis(Axis.Y, _scaleDist);
                        break;
                    }
                    case ScaleMode.ScaleAxisZ:
                    {
                        ScaleObjectAxis(Axis.Z, _scaleDist);
                        break;
                    }
                    case ScaleMode.UniformScale:
                    {
                        UniformScaleObject(_scaleDist);
                        break;
                    }
                }
            }
        }

        public override void OnRepaint()
        {
            switch (_scaleMode)
            {
                case ScaleMode.ScaleAxisX:
                {
                    TransformAxes.GetOrientation(GameObject.transform, TransformSpace.Local, Axis.X,
                        out Vector3 upwards,
                        out _, out _);

                    Handles.color = Handles.xAxisColor;
                    DrawAxisLine(GameObject.transform.position, upwards);

                    break;
                }
                case ScaleMode.ScaleAxisY:
                {
                    TransformAxes.GetOrientation(GameObject.transform, TransformSpace.Local, Axis.Y,
                        out Vector3 upwards,
                        out _, out _);

                    Handles.color = Handles.yAxisColor;
                    DrawAxisLine(GameObject.transform.position, upwards);

                    break;
                }
                case ScaleMode.ScaleAxisZ:
                {
                    TransformAxes.GetOrientation(GameObject.transform, TransformSpace.Local, Axis.Z,
                        out Vector3 upwards,
                        out _, out _);

                    Handles.color = Handles.zAxisColor;
                    DrawAxisLine(GameObject.transform.position, upwards);

                    break;
                }
                case ScaleMode.UniformScale:
                {
                    Handles.color = Handles.selectedColor;
                    Vector3 position = GameObject.transform.position;
                    Handles.RectangleHandleCap(0, position, Quaternion.FromToRotation(Vector3.forward, Vector3.up),
                        HandleUtility.GetHandleSize(position) * 0.5f, EventType.Repaint);

                    break;
                }
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

        private float GetWithBiggestAbsValue()
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

        private void UniformScaleObject(float scale)
        {
            if (EnableSnapScale)
            {
                GameObject.transform.localScale = Snapping.Snap(
                    new Vector3(_startScale.x + scale, _startScale.y + scale, _startScale.z + scale),
                    new Vector3(SnapScale, SnapScale, SnapScale));
            }
            else
            {
                GameObject.transform.localScale = (scale + 1) * _startScale;
            }
        }

        private void ScaleObjectAxis(Axis axis, float scale)
        {
            Vector3 result = GameObject.transform.localScale;

            if (EnableSnapScale)
            {
                switch (axis)
                {
                    case Axis.X:
                    {
                        result.x = Snapping.Snap(_startScale.x + scale, SnapScale);
                        break;
                    }
                    case Axis.Y:
                    {
                        result.y = Snapping.Snap(_startScale.y + scale, SnapScale);
                        break;
                    }
                    case Axis.Z:
                    {
                        result.z = Snapping.Snap(_startScale.z + scale, SnapScale);
                        break;
                    }
                }
            }
            else
            {
                switch (axis)
                {
                    case Axis.X:
                    {
                        result.x = (scale + 1) * _startScale.x;
                        break;
                    }
                    case Axis.Y:
                    {
                        result.y = (scale + 1) * _startScale.y;
                        break;
                    }
                    case Axis.Z:
                    {
                        result.z = (scale + 1) * _startScale.z;
                        break;
                    }
                }
            }

            GameObject.transform.localScale = result;
        }
    }
}
#endif
