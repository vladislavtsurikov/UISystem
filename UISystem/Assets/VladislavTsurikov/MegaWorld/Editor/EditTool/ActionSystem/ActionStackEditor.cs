#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;

namespace VladislavTsurikov.MegaWorld.Editor.EditTool.ActionSystem
{
    public class ActionStackEditor : TabComponentStackEditor<Action, IMGUIElementEditor>
    {
        public ActionStackEditor(ActionStack stack) : base(stack)
        {
            _tabStackEditor.TabWidthFromName = true;
            _tabStackEditor.Draggable = true;
        }

        public void DrawSettings()
        {
            if (Stack.SelectedElement == null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("No Action Selected");
                EditorGUILayout.EndVertical();
            }
            else
            {
                SelectedEditor?.OnGUI();
            }
        }

        public void DrawButtons() => _tabStackEditor.OnGUI();
    }
}
#endif
