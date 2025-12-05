#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.SceneManagerTool.Editor.SettingsSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneTypeSystem;

namespace VladislavTsurikov.SceneManagerTool.Editor.SceneTypeSystem
{
    public class SceneTypeEditor : ReorderableListComponentEditor
    {
        protected SceneType SceneType;
        public SettingsStackEditor SettingsStackEditor;

        public override void OnEnable()
        {
            SceneType = (SceneType)Target;
            SettingsStackEditor = new SettingsStackEditor(new GUIContent("Settings"), false, SceneType.SettingsStack);
        }

        public override void OnGUI(Rect rect, int index)
        {
            SettingsStackEditor.OnGUI(rect);

            rect.y += SettingsStackEditor.GetElementStackHeight();
        }

        public override float GetElementHeight(int index)
        {
            float height = 0;

            height += SettingsStackEditor.GetElementStackHeight();

            return height;
        }
    }
}
#endif
