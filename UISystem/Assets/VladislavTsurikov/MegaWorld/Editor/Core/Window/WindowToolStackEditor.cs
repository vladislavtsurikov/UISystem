#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;

namespace VladislavTsurikov.MegaWorld.Editor.Core.Window
{
    public class WindowToolStackEditor : TabComponentStackEditor<ToolWindow, ToolWindowEditor>
    {
        public WindowToolStackEditor(WindowToolStack stack) : base(stack)
        {
        }

        public override void OnTabStackGUI()
        {
            _tabStackEditor.OnGUI();

            if (Stack.SelectedElement == null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("No Tool Selected");
                EditorGUILayout.EndVertical();
            }
        }
    }
}
#endif
