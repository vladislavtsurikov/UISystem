#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneTypeSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem;

namespace VladislavTsurikov.SceneManagerTool.Editor.SettingsSystem
{
    [ElementEditor(typeof(SceneBehavior))]
    public class SceneBehaviorEditor : ReorderableListComponentEditor
    {
        private SceneBehavior _sceneBehavior;

        public override void OnEnable() => _sceneBehavior = (SceneBehavior)Target;

        public override void OnGUI(Rect rect, int index)
        {
            _sceneBehavior.SceneCloseBehavior = (SceneCloseBehavior)CustomEditorGUI.EnumPopup(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Scene Close Behavior"), _sceneBehavior.SceneCloseBehavior);

            rect.y += CustomEditorGUI.SingleLineHeight;

            _sceneBehavior.SceneOpenBehavior = (SceneOpenBehavior)CustomEditorGUI.EnumPopup(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Scene Open Behavior"), _sceneBehavior.SceneOpenBehavior);

            rect.y += CustomEditorGUI.SingleLineHeight;
        }

        public override float GetElementHeight(int index)
        {
            float height = 0;

            height += CustomEditorGUI.SingleLineHeight;
            height += CustomEditorGUI.SingleLineHeight;

            return height;
        }
    }
}
#endif
