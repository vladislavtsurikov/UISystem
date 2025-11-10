#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.GUI.ModifyTransformComponents
{
    [ElementEditor(typeof(Position))]
    public class PositionEditor : ReorderableListComponentEditor
    {
        private Position _settings;

        public override void OnEnable() => _settings = (Position)Target;

        public override void OnGUI(Rect rect, int index)
        {
            _settings.Strength = EditorGUI.Slider(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Strength",
                    "In which direction and with what force the position offset will act."), _settings.Strength, -1f,
                1f);
            rect.y += EditorGUIUtility.singleLineHeight;
            _settings.YStrengthRandomize = EditorGUI.Slider(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent(
                    "Strength Randomize (%)",
                    "Allows you to set how strong the influence of the random will be."), _settings.YStrengthRandomize,
                0f, 100f);
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index)
        {
            var height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight * 2;

            return height;
        }
    }
}
#endif
