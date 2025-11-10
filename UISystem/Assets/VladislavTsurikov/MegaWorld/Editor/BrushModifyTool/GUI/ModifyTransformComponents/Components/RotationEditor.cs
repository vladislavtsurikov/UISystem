#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.GUI.ModifyTransformComponents
{
    [ElementEditor(typeof(Rotation))]
    public class RotationEditor : ReorderableListComponentEditor
    {
        private Rotation _settings;

        public override void OnEnable() => _settings = (Rotation)Target;

        public override void OnGUI(Rect rect, int index)
        {
            _settings.ModifyStrengthRotation = EditorGUI.Slider(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Strength", "How fast the object will rotate."), _settings.ModifyStrengthRotation, 0f,
                10f);
            rect.y += EditorGUIUtility.singleLineHeight;

            _settings.ModifyRandomRotationX = EditorGUI.Toggle(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Random Rotation X", "Apply random rotation to the axis"),
                _settings.ModifyRandomRotationX);
            rect.y += EditorGUIUtility.singleLineHeight;
            {
                ++EditorGUI.indentLevel;

                if (_settings.ModifyRandomRotationX)
                {
                    _settings.ModifyRandomRotationValues.x = EditorGUI.Slider(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        new GUIContent("Randomize X",
                            "Allows you to set how strong the influence of the random will be"),
                        _settings.ModifyRandomRotationValues.x, 0.0f, 1.0f);
                    rect.y += EditorGUIUtility.singleLineHeight;
                }
                else
                {
                    _settings.ModifyRotationValues.x = EditorGUI.Slider(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        new GUIContent("Rotation X", "Allows you to select positive or negative rotation."),
                        _settings.ModifyRotationValues.x, -1.0f, 1.0f);
                    rect.y += EditorGUIUtility.singleLineHeight;
                }

                --EditorGUI.indentLevel;
            }

            _settings.ModifyRandomRotationY = EditorGUI.Toggle(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Random Rotation Y", "Apply random rotation to the axis"),
                _settings.ModifyRandomRotationY);
            rect.y += EditorGUIUtility.singleLineHeight;
            {
                ++EditorGUI.indentLevel;

                if (_settings.ModifyRandomRotationY)
                {
                    _settings.ModifyRandomRotationValues.y = EditorGUI.Slider(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        new GUIContent("Randomize Y",
                            "Allows you to set how strong the influence of the random will be"),
                        _settings.ModifyRandomRotationValues.y, 0.0f, 1.0f);
                    rect.y += EditorGUIUtility.singleLineHeight;
                }
                else
                {
                    _settings.ModifyRotationValues.y = EditorGUI.Slider(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        new GUIContent("Rotation Y", "Allows you to select positive or negative rotation."),
                        _settings.ModifyRotationValues.y, -1.0f, 1.0f);
                    rect.y += EditorGUIUtility.singleLineHeight;
                }

                --EditorGUI.indentLevel;
            }

            _settings.ModifyRandomRotationZ = EditorGUI.Toggle(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Random Rotation Z", "Apply random rotation to the axis"),
                _settings.ModifyRandomRotationZ);
            rect.y += EditorGUIUtility.singleLineHeight;
            {
                ++EditorGUI.indentLevel;

                if (_settings.ModifyRandomRotationZ)
                {
                    _settings.ModifyRandomRotationValues.z = EditorGUI.Slider(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        new GUIContent("Randomize Z",
                            "Allows you to set how strong the influence of the random will be"),
                        _settings.ModifyRandomRotationValues.z, 0.0f, 1.0f);
                    rect.y += EditorGUIUtility.singleLineHeight;
                }
                else
                {
                    _settings.ModifyRotationValues.z = EditorGUI.Slider(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        new GUIContent("Rotation Z", "Allows you to select positive or negative rotation."),
                        _settings.ModifyRotationValues.z, -1.0f, 1.0f);
                    rect.y += EditorGUIUtility.singleLineHeight;
                }

                --EditorGUI.indentLevel;
            }
        }

        public override float GetElementHeight(int index)
        {
            var height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;
            {
                ++EditorGUI.indentLevel;

                if (_settings.ModifyRandomRotationX)
                {
                    height += EditorGUIUtility.singleLineHeight;
                }
                else
                {
                    height += EditorGUIUtility.singleLineHeight;
                }

                --EditorGUI.indentLevel;
            }

            height += EditorGUIUtility.singleLineHeight;
            {
                if (_settings.ModifyRandomRotationY)
                {
                    height += EditorGUIUtility.singleLineHeight;
                }
                else
                {
                    height += EditorGUIUtility.singleLineHeight;
                }
            }

            height += EditorGUIUtility.singleLineHeight;
            {
                if (_settings.ModifyRandomRotationZ)
                {
                    height += EditorGUIUtility.singleLineHeight;
                }
                else
                {
                    height += EditorGUIUtility.singleLineHeight;
                }
            }

            return height;
        }
    }
}
#endif
