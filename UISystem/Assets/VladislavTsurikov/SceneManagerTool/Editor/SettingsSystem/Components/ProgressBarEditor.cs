#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem;

namespace VladislavTsurikov.SceneManagerTool.Editor.SettingsSystem
{
    [ElementEditor(typeof(ProgressBar))]
    public class ProgressBarEditor : ReorderableListComponentEditor
    {
        private ProgressBar _progressBar;

        public override void OnEnable() => _progressBar = (ProgressBar)Target;

        public override void OnGUI(Rect rect, int index)
        {
            _progressBar.SceneReference.SceneAsset = (SceneAsset)CustomEditorGUI.ObjectField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent(""),
                _progressBar.SceneReference.SceneAsset, typeof(SceneAsset));
            rect.y += CustomEditorGUI.SingleLineHeight;

            _progressBar.DisableFade =
                CustomEditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Disable Fade"), _progressBar.DisableFade);
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
