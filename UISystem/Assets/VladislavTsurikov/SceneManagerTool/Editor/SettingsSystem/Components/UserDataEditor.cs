#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem;

namespace VladislavTsurikov.SceneManagerTool.Editor.SettingsSystem
{
    [ElementEditor(typeof(UserData))]
    public class UserDataEditor : ReorderableListComponentEditor
    {
        private UserData _userData;

        public override void OnEnable() => _userData = (UserData)Target;

        public override void OnGUI(Rect rect, int index) =>
            _userData.ScriptableObject = (ScriptableObject)CustomEditorGUI.ObjectField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                null, _userData.ScriptableObject, typeof(ScriptableObject));

        public override float GetElementHeight(int index)
        {
            float height = 0;

            height += CustomEditorGUI.SingleLineHeight;

            return height;
        }
    }
}
#endif
