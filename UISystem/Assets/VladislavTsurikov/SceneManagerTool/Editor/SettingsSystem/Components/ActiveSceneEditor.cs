#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem;

namespace VladislavTsurikov.SceneManagerTool.Editor.SettingsSystem
{
    [ElementEditor(typeof(ActiveScene))]
    public class ActiveSceneEditor : ReorderableListComponentEditor
    {
        private ActiveScene _activeScene;

        public override void OnEnable() => _activeScene = (ActiveScene)Target;

        public override void OnGUI(Rect rect, int index)
        {
            _activeScene.SceneReference.SceneAsset = (SceneAsset)CustomEditorGUI.ObjectField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), null,
                _activeScene.SceneReference.SceneAsset, typeof(SceneAsset));
            rect.y += CustomEditorGUI.SingleLineHeight;
        }

        public override float GetElementHeight(int index)
        {
            float height = 0;

            height += CustomEditorGUI.SingleLineHeight;

            return height;
        }
    }
}
#endif
