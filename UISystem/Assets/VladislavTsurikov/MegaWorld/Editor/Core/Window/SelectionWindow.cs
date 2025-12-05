#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor;

namespace VladislavTsurikov.MegaWorld.Editor.Core.Window
{
    public class SelectionWindow : BaseWindow<SelectionWindow>
    {
        protected override void OnGUI()
        {
            base.OnGUI();

            WindowData.Instance.SelectionData.DeleteNullElements();
            WindowData.Instance.SelectedData.SetSelectedData();

            OnMainGUI();
        }

        private void OnMainGUI()
        {
            GUILayout.Space(5);

            ToolWindowEditor toolWindowEditor = WindowData.Instance.WindowToolStackEditor.SelectedEditor;

            if (toolWindowEditor != null)
            {
                WindowData.Instance.SelectionData.OnGUI(toolWindowEditor.SelectionDataDrawer,
                    toolWindowEditor.Target.GetType());
            }
        }
    }
}
#endif
