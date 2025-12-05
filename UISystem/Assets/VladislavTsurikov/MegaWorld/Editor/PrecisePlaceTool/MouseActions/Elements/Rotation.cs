#if UNITY_EDITOR
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColorUtility.Runtime;
using VladislavTsurikov.EditorShortcutCombo.Editor;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.MouseActions
{
    public enum RotationMode
    {
        RotateAroundX,
        RotateAroundY,
        RotateAroundZ,
        FreeRotating,
        ScreenRotating
    }

    public enum WayRotateY
    {
        Direction,
        Offset
    }

    [Name("Rotation")]
    public class Rotation : MouseAction
    {
        private Quaternion _сurrentRotation;
        private float _angle;
        private Vector3 _forward;
        private ShortcutCombo _mouseFreeRotate;
        private ShortcutCombo _mouseFreeScreen;
        private ShortcutCombo _mouseRotateAroundX;
        private ShortcutCombo _mouseRotateAroundY;
        private ShortcutCombo _mouseRotateAroundZ;
        private Vector3 _right;
        private Vector3 _rotationAxis;
        private float _rotationDist;
        private RotationMode _rotationMode;
        private Quaternion _startRotation;
        private Vector3 _upwards;
        public bool EnableSnapRotate = false;

        public MouseSensitivitySettings MouseRotationSettings = new();
        public float SnapRotate = 15f;
        public WayRotateY WayRotateY = WayRotateY.Offset;

        [OnDeserializing]
        private void OnDeserializing() => InitShortcutCombo();

        protected override void OnCreate() => InitShortcutCombo();

        private void InitShortcutCombo()
        {
            _mouseRotateAroundY = new ShortcutCombo();
            _mouseRotateAroundY.AddKey(KeyCode.LeftShift);

            _mouseRotateAroundX = new ShortcutCombo();
            _mouseRotateAroundX.AddKey(KeyCode.X);
            _mouseRotateAroundX.AddShortcutCombo(_mouseRotateAroundY);

            _mouseRotateAroundZ = new ShortcutCombo();
            _mouseRotateAroundZ.AddKey(KeyCode.Z);
            _mouseRotateAroundZ.AddShortcutCombo(_mouseRotateAroundY);

            _mouseFreeRotate = new ShortcutCombo();
            _mouseFreeRotate.AddKey(KeyCode.F);
            _mouseFreeRotate.AddShortcutCombo(_mouseRotateAroundY);

            _mouseFreeScreen = new ShortcutCombo();
            _mouseFreeScreen.AddKey(KeyCode.S);
            _mouseFreeScreen.AddShortcutCombo(_mouseRotateAroundY);
        }

        public override void CheckShortcutCombos(GameObject gameObject, Vector3 normal)
        {
            if (_mouseRotateAroundY.IsActive())
            {
                if (Begin(gameObject, normal) || _rotationMode != RotationMode.RotateAroundY)
                {
                    SetStartValue(gameObject);
                    _rotationAxis = TransformAxes.GetVector(Axis.Y,
                        GlobalCommonComponentSingleton<TransformSpaceSettings>.Instance.TransformSpace,
                        GameObject.transform);
                }

                _rotationMode = RotationMode.RotateAroundY;
            }
            else if (_mouseRotateAroundX.IsActive())
            {
                if (Begin(gameObject, normal) || _rotationMode != RotationMode.RotateAroundX)
                {
                    SetStartValue(gameObject);
                    _rotationAxis = TransformAxes.GetVector(Axis.X,
                        GlobalCommonComponentSingleton<TransformSpaceSettings>.Instance.TransformSpace,
                        GameObject.transform);
                }

                _rotationMode = RotationMode.RotateAroundX;
            }
            else if (_mouseRotateAroundZ.IsActive())
            {
                if (Begin(gameObject, normal) || _rotationMode != RotationMode.RotateAroundZ)
                {
                    SetStartValue(gameObject);
                    _rotationAxis = TransformAxes.GetVector(Axis.Z,
                        GlobalCommonComponentSingleton<TransformSpaceSettings>.Instance.TransformSpace,
                        GameObject.transform);
                }

                _rotationMode = RotationMode.RotateAroundZ;
            }
            else if (_mouseFreeRotate.IsActive())
            {
                if (Begin(gameObject, normal) || _rotationMode != RotationMode.FreeRotating)
                {
                    SetStartValue(gameObject);
                }

                _rotationMode = RotationMode.FreeRotating;
            }
            else if (_mouseFreeScreen.IsActive())
            {
                if (Begin(gameObject, normal) || _rotationMode != RotationMode.ScreenRotating)
                {
                    SetStartValue(gameObject);
                    _rotationAxis = SceneView.currentDrawingSceneView.camera.transform.forward;
                }

                _rotationMode = RotationMode.ScreenRotating;
            }
            else
            {
                End();
            }
        }

        public override void OnMouseMove()
        {
            switch (_rotationMode)
            {
                case RotationMode.RotateAroundX:
                {
                    RotateObjectAroundAxis(_rotationAxis, EnableSnapRotate);
                    break;
                }
                case RotationMode.RotateAroundY:
                {
                    if (WayRotateY == WayRotateY.Offset)
                    {
                        RotateObjectAroundAxis(_rotationAxis, EnableSnapRotate);
                    }
                    else
                    {
                        RotateToMouseForward();
                    }

                    break;
                }
                case RotationMode.RotateAroundZ:
                {
                    RotateObjectAroundAxis(_rotationAxis, EnableSnapRotate);
                    break;
                }
                case RotationMode.FreeRotating:
                {
                    FreeRotateObject();
                    break;
                }
                case RotationMode.ScreenRotating:
                {
                    RotateObjectAroundAxis(_rotationAxis, false);
                    break;
                }
            }
        }

        private void SetStartValue(GameObject gameObject)
        {
            GameObject = gameObject;
            _rotationDist = 0;
            _startRotation = gameObject.transform.rotation;
            _сurrentRotation = Quaternion.identity;

            _upwards = gameObject.transform.up;
            TransformAxes.GetRightForward(_upwards, out _right, out _forward);
        }

        public override void OnRepaint()
        {
            switch (_rotationMode)
            {
                case RotationMode.RotateAroundX:
                {
                    TransformAxes.GetOrientation(GameObject.transform,
                        GlobalCommonComponentSingleton<TransformSpaceSettings>.Instance.TransformSpace, Axis.X,
                        out Vector3 upwards, out _, out Vector3 forward);

                    Handles.color = Handles.xAxisColor;
                    Handles.CircleHandleCap(
                        0,
                        GameObject.transform.position,
                        Quaternion.LookRotation(upwards),
                        HandleUtility.GetHandleSize(GameObject.transform.position),
                        EventType.Repaint
                    );

                    Handles.color = Handles.selectedColor;
#if UNITY_2020_3_OR_NEWER
                    Handles.DrawLine(GameObject.transform.position,
                        GameObject.transform.position +
                        forward * HandleUtility.GetHandleSize(GameObject.transform.position), 1.0f);
#else
                    UnityEditor.Handles.DrawLine(_gameObject.transform.position, _gameObject.transform.position + forward * HandleUtility.GetHandleSize(_gameObject.transform.position));
#endif
                    Handles.DrawDottedLine(GameObject.transform.position,
                        GameObject.transform.position + _сurrentRotation * forward *
                        HandleUtility.GetHandleSize(GameObject.transform.position), 4.0f);

                    break;
                }
                case RotationMode.RotateAroundY:
                {
                    if (WayRotateY == WayRotateY.Offset)
                    {
                        TransformAxes.GetOrientation(GameObject.transform,
                            GlobalCommonComponentSingleton<TransformSpaceSettings>.Instance.TransformSpace, Axis.Y,
                            out Vector3 upwards, out _, out Vector3 forward);

                        Handles.color = Handles.yAxisColor;
                        Handles.CircleHandleCap(
                            0,
                            GameObject.transform.position,
                            Quaternion.LookRotation(upwards),
                            HandleUtility.GetHandleSize(GameObject.transform.position),
                            EventType.Repaint
                        );

                        Handles.color = Handles.selectedColor;
#if UNITY_2020_3_OR_NEWER
                        Handles.DrawLine(GameObject.transform.position,
                            GameObject.transform.position +
                            forward * HandleUtility.GetHandleSize(GameObject.transform.position), 1.0f);
#else
                        UnityEditor.Handles.DrawLine(_gameObject.transform.position, _gameObject.transform.position + forward * HandleUtility.GetHandleSize(_gameObject.transform.position));
#endif
                        Handles.DrawDottedLine(GameObject.transform.position,
                            GameObject.transform.position + _сurrentRotation * forward *
                            HandleUtility.GetHandleSize(GameObject.transform.position), 4.0f);
                    }
                    else
                    {
                        if (IntersectsHitPlane(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition),
                                out Vector3 point))
                        {
                            Handles.DrawDottedLine(GameObject.transform.position, point, 4.0f);
                        }
                    }

                    break;
                }
                case RotationMode.RotateAroundZ:
                {
                    TransformAxes.GetOrientation(GameObject.transform,
                        GlobalCommonComponentSingleton<TransformSpaceSettings>.Instance.TransformSpace, Axis.Z,
                        out Vector3 upwards, out _, out Vector3 forward);

                    Handles.color = Handles.zAxisColor;
                    Handles.CircleHandleCap(
                        0,
                        GameObject.transform.position,
                        Quaternion.LookRotation(upwards),
                        HandleUtility.GetHandleSize(GameObject.transform.position),
                        EventType.Repaint
                    );

                    Handles.color = Handles.selectedColor;
#if UNITY_2020_3_OR_NEWER
                    Handles.DrawLine(GameObject.transform.position,
                        GameObject.transform.position +
                        forward * HandleUtility.GetHandleSize(GameObject.transform.position), 1.0f);
#else
                    UnityEditor.Handles.DrawLine(_gameObject.transform.position, _gameObject.transform.position + forward * HandleUtility.GetHandleSize(_gameObject.transform.position));
#endif
                    Handles.DrawDottedLine(GameObject.transform.position,
                        GameObject.transform.position + _сurrentRotation * forward *
                        HandleUtility.GetHandleSize(GameObject.transform.position), 4.0f);

                    break;
                }
                case RotationMode.FreeRotating:
                {
                    Vector3 upwards = SceneView.currentDrawingSceneView.camera.transform.forward;

                    TransformAxes.GetRightForward(upwards, out _, out _);

                    Handles.color = Handles.selectedColor.WithAlpha(0.5f);
                    Handles.CircleHandleCap(
                        0,
                        GameObject.transform.position,
                        Quaternion.LookRotation(upwards),
                        HandleUtility.GetHandleSize(GameObject.transform.position),
                        EventType.Repaint
                    );

                    Vector3 axisX = TransformAxes.GetVector(Axis.X, TransformSpace.Local, GameObject.transform);
                    Vector3 axisY = TransformAxes.GetVector(Axis.Y, TransformSpace.Local, GameObject.transform);
                    Vector3 axisZ = TransformAxes.GetVector(Axis.Z, TransformSpace.Local, GameObject.transform);

                    Handles.color = Handles.xAxisColor.WithAlpha(0.5f);
                    Handles.CircleHandleCap(
                        0,
                        GameObject.transform.position,
                        Quaternion.LookRotation(axisX),
                        HandleUtility.GetHandleSize(GameObject.transform.position),
                        EventType.Repaint
                    );

                    Handles.color = Handles.yAxisColor.WithAlpha(0.5f);
                    Handles.CircleHandleCap(
                        0,
                        GameObject.transform.position,
                        Quaternion.LookRotation(axisY),
                        HandleUtility.GetHandleSize(GameObject.transform.position),
                        EventType.Repaint
                    );

                    Handles.color = Handles.zAxisColor.WithAlpha(0.5f);
                    Handles.CircleHandleCap(
                        0,
                        GameObject.transform.position,
                        Quaternion.LookRotation(axisZ),
                        HandleUtility.GetHandleSize(GameObject.transform.position),
                        EventType.Repaint
                    );

                    break;
                }
                case RotationMode.ScreenRotating:
                {
                    Vector3 upwards = SceneView.currentDrawingSceneView.camera.transform.forward;

                    TransformAxes.GetRightForward(upwards, out _, out _);

                    Handles.color = Color.white;
                    Handles.CircleHandleCap(
                        0,
                        GameObject.transform.position,
                        Quaternion.LookRotation(upwards),
                        HandleUtility.GetHandleSize(GameObject.transform.position),
                        EventType.Repaint
                    );

                    break;
                }
            }
        }

        private void RotateToMouseForward()
        {
            if (IntersectsHitPlane(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out Vector3 point))
            {
                Vector3 vector = point - GameObject.transform.position;
                var vectorLength = vector.magnitude;

                if (vectorLength < 0.01f)
                {
                    vector = Vector3.up * 0.01f;
                    vectorLength = 0.01f;
                }

                _angle = Vector3.Angle(_forward, vector.normalized);
                if (Vector3.Dot(vector.normalized, _right) < 0.0f)
                {
                    _angle = -_angle;
                }

                if (EnableSnapRotate)
                {
                    _angle = Snapping.Snap(_angle, SnapRotate);
                }

                GameObject.transform.rotation =
                    GetRotation(new Vector3(0, _angle, 0));
            }
        }

        private Quaternion GetRotation(Vector3 euler)
        {
            var placeOrientation = Quaternion.LookRotation(_forward, _upwards);
            return placeOrientation * Quaternion.Euler(euler);
        }

        private void RotateObjectAroundAxis(Vector3 rotationAxis, bool snapRotate)
        {
            _rotationDist += Event.current.delta.x * MouseRotationSettings.MouseSensitivity * 2f;
            var angle = _rotationDist;
            if (snapRotate)
            {
                angle = Snapping.Snap(_rotationDist, SnapRotate);
            }

            _сurrentRotation = Quaternion.AngleAxis(angle, rotationAxis);
            GameObject.transform.rotation = _сurrentRotation * _startRotation;
        }

        private void FreeRotateObject()
        {
            var rotationAmountInDegreesX = -Event.current.delta.x * MouseRotationSettings.MouseSensitivity * 2f;
            var rotationAmountInDegreesY = -Event.current.delta.y * MouseRotationSettings.MouseSensitivity * 2f;

            var rotationX = Quaternion.AngleAxis(rotationAmountInDegreesY,
                SceneView.currentDrawingSceneView.camera.transform.right);
            var rotationY = Quaternion.AngleAxis(rotationAmountInDegreesX,
                SceneView.currentDrawingSceneView.camera.transform.up);

            _сurrentRotation = rotationX * rotationY;

            GameObject.transform.rotation = _сurrentRotation * GameObject.transform.rotation;
        }

        private bool IntersectsHitPlane(Ray ray, out Vector3 hitPoint)
        {
            var plane = new Plane(_upwards, GameObject.transform.position);
            if (plane.Raycast(ray, out var rayDistance))
            {
                hitPoint = ray.GetPoint(rayDistance);
                return true;
            }

            hitPoint = Vector3.zero;
            return false;
        }
    }
}
#endif
