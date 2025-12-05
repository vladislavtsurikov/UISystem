#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.TransformElementSystem
{
    [ElementEditor(typeof(Rotation))]
    public class RotationEditor : ReorderableListComponentEditor
    {
        private Rotation _rotation;

        public override void OnEnable() => _rotation = (Rotation)Target;

        public override void OnGUI(Rect rect, int index)
        {
            _rotation.RandomizeOrientationX = EditorGUI.Slider(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Randomize X (%)"), _rotation.RandomizeOrientationX, 0.0f, 100.0f);
            rect.y += EditorGUIUtility.singleLineHeight;
            _rotation.RandomizeOrientationY = EditorGUI.Slider(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Randomize Y (%)"), _rotation.RandomizeOrientationY, 0.0f, 100.0f);
            rect.y += EditorGUIUtility.singleLineHeight;
            _rotation.RandomizeOrientationZ = EditorGUI.Slider(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Randomize Z (%)"), _rotation.RandomizeOrientationZ, 0.0f, 100.0f);
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index)
        {
            var height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;

            return height;
        }
    }
}
#endif
