#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.TransformElementSystem
{
    [ElementEditor(typeof(SlopePosition))]
    public class SlopePositionEditor : ReorderableListComponentEditor
    {
        private SlopePosition _slopePosition;

        public override void OnEnable() => _slopePosition = (SlopePosition)Target;

        public override void OnGUI(Rect rect, int index)
        {
            _slopePosition.MaxSlope =
                EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Max Slope"), _slopePosition.MaxSlope, 20, 90);
            rect.y += EditorGUIUtility.singleLineHeight;
            _slopePosition.PositionOffsetY = EditorGUI.FloatField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Position Offset Y"), _slopePosition.PositionOffsetY);
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
