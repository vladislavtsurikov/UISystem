#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.TransformElementSystem
{
    [ElementEditor(typeof(SnapRotation))]
    public class SnapRotationEditor : ReorderableListComponentEditor
    {
        private SnapRotation _snapRotation;

        public override void OnEnable() => _snapRotation = (SnapRotation)Target;

        public override void OnGUI(Rect rect, int index)
        {
            _snapRotation.RotateAxisX = EditorGUI.Toggle(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Rotate Axis X"), _snapRotation.RotateAxisX);
            rect.y += EditorGUIUtility.singleLineHeight;
            _snapRotation.RotateAxisY = EditorGUI.Toggle(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Rotate Axis Y"), _snapRotation.RotateAxisY);
            rect.y += EditorGUIUtility.singleLineHeight;
            _snapRotation.RotateAxisZ = EditorGUI.Toggle(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Rotate Axis Z"), _snapRotation.RotateAxisZ);
            rect.y += EditorGUIUtility.singleLineHeight;
            _snapRotation.SnapRotationAngle = EditorGUI.FloatField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Snap Rotation Angle"), _snapRotation.SnapRotationAngle);
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index)
        {
            var height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;

            return height;
        }
    }
}
#endif
