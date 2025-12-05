#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.SceneManagerTool.Editor.SceneTypeSystem;
using VladislavTsurikov.SceneManagerTool.Editor.SettingsSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;

namespace VladislavTsurikov.SceneManagerTool.Editor.SceneCollectionSystem
{
    [ElementEditor(typeof(SceneCollection))]
    public class SceneCollectionEditor : ReorderableListComponentEditor
    {
        private SceneCollection _sceneCollection;
        private SceneTypeComponentStackEditor _sceneTypeComponentStackEditor;

        public SettingsStackEditor SettingsStackEditor;

        public override void OnEnable()
        {
            _sceneCollection = (SceneCollection)Target;
            _sceneTypeComponentStackEditor = new SceneTypeComponentStackEditor(new GUIContent("Scenes"),
                _sceneCollection.SceneTypeComponentStack);
            SettingsStackEditor =
                new SettingsStackEditor(new GUIContent("Settings"), true, _sceneCollection.SettingsStack);
        }

        public override void OnGUI(Rect rect, int index)
        {
            _sceneCollection.Startup =
                CustomEditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Startup"), _sceneCollection.Startup);
            rect.y += CustomEditorGUI.SingleLineHeight;

            SettingsStackEditor.OnGUI(rect);

            rect.y += SettingsStackEditor.GetElementStackHeight();

            _sceneTypeComponentStackEditor.OnGUI(rect);
        }

        public override float GetElementHeight(int index)
        {
            float height = 0;

            height += CustomEditorGUI.SingleLineHeight;
            height += _sceneTypeComponentStackEditor.GetElementStackHeight();
            height += SettingsStackEditor.GetElementStackHeight();

            return height;
        }
    }
}
#endif
