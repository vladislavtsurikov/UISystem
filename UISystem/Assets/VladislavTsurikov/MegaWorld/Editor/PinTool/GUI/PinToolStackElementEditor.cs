#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.Math.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.PinTool.GUI
{
    [DontDrawFoldout]
    [ElementEditor(typeof(PinToolSettings))]
    public class PinToolStackElementEditor : IMGUIElementEditor
    {
        private PinToolSettings _pinToolSettings;

        private bool _positionFoldout = true;
        private bool _rotationFoldout = true;
        private bool _scaleFoldout = true;

        public override void OnEnable() => _pinToolSettings = (PinToolSettings)Target;

        public override void OnGUI()
        {
            _positionFoldout = CustomEditorGUILayout.Foldout(_positionFoldout, "Position");

            if (_positionFoldout)
            {
                EditorGUI.indentLevel++;

                _pinToolSettings.Offset =
                    CustomEditorGUILayout.FloatField(new GUIContent("Offset"), _pinToolSettings.Offset);

                EditorGUI.indentLevel--;
            }

            _rotationFoldout = CustomEditorGUILayout.Foldout(_rotationFoldout, "Rotation");

            if (_rotationFoldout)
            {
                EditorGUI.indentLevel++;

                _pinToolSettings.RotationTransformMode =
                    (TransformMode)CustomEditorGUILayout.EnumPopup(new GUIContent("Transform Mode"),
                        _pinToolSettings.RotationTransformMode);

                switch (_pinToolSettings.RotationTransformMode)
                {
                    case TransformMode.Fixed:
                    {
                        EditorGUI.indentLevel++;

                        _pinToolSettings.FixedRotationValue =
                            CustomEditorGUILayout.Vector3Field(new GUIContent("Rotation"),
                                _pinToolSettings.FixedRotationValue);

                        EditorGUI.indentLevel--;

                        break;
                    }
                    case TransformMode.Snap:
                    {
                        EditorGUI.indentLevel++;

                        _pinToolSettings.SnapRotationValue = Mathf.Max(
                            CustomEditorGUILayout.FloatField(new GUIContent("Rotation Angle"),
                                _pinToolSettings.SnapRotationValue), 0.001f);

                        EditorGUI.indentLevel--;

                        break;
                    }
                }

                if (_pinToolSettings.RotationTransformMode != TransformMode.Fixed)
                {
                    _pinToolSettings.FromDirection =
                        (FromDirection)CustomEditorGUILayout.EnumPopup(new GUIContent("Up"),
                            _pinToolSettings.FromDirection);

                    if (_pinToolSettings.FromDirection == FromDirection.SurfaceNormal)
                    {
                        EditorGUI.indentLevel++;

                        _pinToolSettings.WeightToNormal =
                            CustomEditorGUILayout.Slider(new GUIContent("Weight To Normal"),
                                _pinToolSettings.WeightToNormal, 0, 1);

                        EditorGUI.indentLevel--;
                    }
                }

                EditorGUI.indentLevel--;
            }

            _scaleFoldout = CustomEditorGUILayout.Foldout(_scaleFoldout, "Scale");

            if (_scaleFoldout)
            {
                EditorGUI.indentLevel++;

                _pinToolSettings.ScaleTransformMode =
                    (TransformMode)CustomEditorGUILayout.EnumPopup(new GUIContent("Transform Mode"),
                        _pinToolSettings.ScaleTransformMode);

                switch (_pinToolSettings.ScaleTransformMode)
                {
                    case TransformMode.Fixed:
                    {
                        EditorGUI.indentLevel++;

                        _pinToolSettings.FixedScaleValue = CustomEditorGUILayout.Vector3Field(new GUIContent("Scale"),
                            _pinToolSettings.FixedScaleValue);

                        EditorGUI.indentLevel--;

                        break;
                    }
                    case TransformMode.Snap:
                    {
                        EditorGUI.indentLevel++;

                        _pinToolSettings.SnapScaleValue =
                            Mathf.Max(
                                CustomEditorGUILayout.FloatField(new GUIContent("Snap Scale Value"),
                                    _pinToolSettings.SnapScaleValue), 0.001f);

                        EditorGUI.indentLevel--;

                        break;
                    }
                }

                EditorGUI.indentLevel--;
            }
        }
    }
}
#endif
