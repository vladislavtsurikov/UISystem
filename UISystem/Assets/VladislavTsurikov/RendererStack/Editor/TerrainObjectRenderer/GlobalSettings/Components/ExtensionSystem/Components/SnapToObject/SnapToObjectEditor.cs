#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;

namespace VladislavTsurikov.RendererStack.Editor.TerrainObjectRenderer.GlobalSettings.ExtensionSystem.SnapToObject
{
    [ElementEditor(typeof(Runtime.TerrainObjectRenderer.GlobalSettings.ExtensionSystem.SnapToObject.SnapToObject))]
    public class SnapToObjectEditor : ReorderableListComponentEditor
    {
        private bool _selectAdvancedSettingsFoldout = true;
        private Runtime.TerrainObjectRenderer.GlobalSettings.ExtensionSystem.SnapToObject.SnapToObject _snapToObject;

        public override void OnEnable() =>
            _snapToObject =
                (Runtime.TerrainObjectRenderer.GlobalSettings.ExtensionSystem.SnapToObject.SnapToObject)Target;

        public override void OnGUI(Rect rect, int index)
        {
            _snapToObject.Layers = CustomEditorGUI.LayerField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Layers"),
                _snapToObject.Layers);
            rect.y += CustomEditorGUI.SingleLineHeight;

            _snapToObject.RaycastPositionOffset = CustomEditorGUI.FloatField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Raycast Position Offset"), _snapToObject.RaycastPositionOffset);
            rect.y += CustomEditorGUI.SingleLineHeight;

            _selectAdvancedSettingsFoldout = CustomEditorGUI.Foldout(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Advanced Settings",
                _selectAdvancedSettingsFoldout);
            rect.y += CustomEditorGUI.SingleLineHeight;

            if (_selectAdvancedSettingsFoldout)
            {
                EditorGUI.indentLevel++;

                _snapToObject.MaxRayDistance = CustomEditorGUI.FloatField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Max Ray Distance", "The max distance the ray should check for collisions."),
                    _snapToObject.MaxRayDistance);
                rect.y += CustomEditorGUI.SingleLineHeight;
                _snapToObject.SpawnCheckOffset = CustomEditorGUI.FloatField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Spawn Check Offset", "Raises the ray from the spawn point."),
                    _snapToObject.SpawnCheckOffset);
                rect.y += CustomEditorGUI.SingleLineHeight;

                EditorGUI.indentLevel--;
            }

            if (CustomEditorGUI.ClickButton(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    "Snap To Object"))
            {
                _snapToObject.Snap();
            }
        }

        public override float GetElementHeight(int index)
        {
            float height = 0;

            height += CustomEditorGUI.SingleLineHeight;
            height += CustomEditorGUI.SingleLineHeight;
            height += CustomEditorGUI.SingleLineHeight;

            if (_selectAdvancedSettingsFoldout)
            {
                height += CustomEditorGUI.SingleLineHeight;
                height += CustomEditorGUI.SingleLineHeight;
            }

            height += CustomEditorGUI.SingleLineHeight;

            return height;
        }
    }
}
#endif
