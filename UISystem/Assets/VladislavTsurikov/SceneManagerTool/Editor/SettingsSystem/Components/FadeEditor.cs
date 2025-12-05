#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem;

namespace VladislavTsurikov.SceneManagerTool.Editor.SettingsSystem
{
    [ElementEditor(typeof(FadeTransition))]
    public class FadeEditor : ReorderableListComponentEditor
    {
        private FadeTransition _component;

        public override void OnEnable() => _component = (FadeTransition)Target;

        public override void OnGUI(Rect rect, int index)
        {
            _component.SceneReference.SceneAsset = (SceneAsset)CustomEditorGUI.ObjectField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent(""),
                _component.SceneReference.SceneAsset, typeof(SceneAsset));
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
