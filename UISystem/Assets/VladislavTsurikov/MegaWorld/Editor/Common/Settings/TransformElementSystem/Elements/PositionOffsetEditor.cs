#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.TransformElementSystem
{
    [ElementEditor(typeof(PositionOffset))]
    public class PositionOffsetEditor : ReorderableListComponentEditor
    {
        private PositionOffset _positionOffset;

        public override void OnEnable() => _positionOffset = (PositionOffset)Target;

        public override void OnGUI(Rect rect, int index)
        {
            _positionOffset.MinPositionOffsetY = EditorGUI.FloatField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Min Position Offset Y"), _positionOffset.MinPositionOffsetY);
            rect.y += EditorGUIUtility.singleLineHeight;
            _positionOffset.MaxPositionOffsetY = EditorGUI.FloatField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Max Position Offset Y"), _positionOffset.MaxPositionOffsetY);
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index)
        {
            var height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;

            return height;
        }
    }
}
#endif
