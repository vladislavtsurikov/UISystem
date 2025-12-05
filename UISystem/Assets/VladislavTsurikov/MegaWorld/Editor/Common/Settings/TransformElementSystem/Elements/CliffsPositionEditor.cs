#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.TransformElementSystem
{
    [ElementEditor(typeof(CliffsPosition))]
    public class CliffsPositionEditor : ReorderableListComponentEditor
    {
        private CliffsPosition _cliffsPosition;

        public override void OnEnable() => _cliffsPosition = (CliffsPosition)Target;

        public override void OnGUI(Rect rect, int index)
        {
            _cliffsPosition.OffsetPosition = EditorGUI.FloatField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Additional Rotation"), _cliffsPosition.OffsetPosition);
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index)
        {
            var height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;

            return height;
        }
    }
}
#endif
