#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.IMGUIUtility.Editor;

namespace VladislavTsurikov.MegaWorld.Editor.Core.Window
{
    public class ToolsWindow : BaseWindow<ToolsWindow>
    {
        protected override void OnGUI()
        {
            base.OnGUI();

            EditorGUI.indentLevel = 0;

            CustomEditorGUILayout.IsInspector = false;

            WindowData.Instance.WindowToolStackEditor.OnTabStackGUI();
        }
    }
}
#endif
