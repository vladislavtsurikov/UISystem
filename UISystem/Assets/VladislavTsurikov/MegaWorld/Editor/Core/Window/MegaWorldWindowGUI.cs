#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.EditorShortcutCombo.Editor;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings;

namespace VladislavTsurikov.MegaWorld.Editor.Core.Window
{
    public partial class MegaWorldWindow
    {
        protected override void OnGUI()
        {
            base.OnGUI();

            WindowData.Instance.SelectionData.DeleteNullElements();
            WindowData.Instance.SelectedData.SetSelectedData();
            UpdateSceneViewEvent();

            OnMainGUI();
        }

        [MenuItem(MenuUtils.MegaWorldWindowMenuItemItemName, false, 0)]
        private static void OpenMegaWorldWindow()
        {
            if (ToolsWindow.IsOpen)
            {
                ToolsWindow.Instance.Close();
            }

            if (SelectionWindow.IsOpen)
            {
                SelectionWindow.Instance.Close();
            }

            OpenWindow("Mega World");
        }

        [MenuItem(MenuUtils.SeparateWindowsMenuItemItemName, false, 1)]
        private static void OpenSeparateWindows()
        {
            OpenWindow("Mega World");
            ToolsWindow.OpenWindow("Tools");
            SelectionWindow.OpenWindow("Selection");
        }

        [MenuItem("Window/Vladislav Tsurikov/Mega World/Documentation", false, 1000)]
        public static void Documentation() =>
            Application.OpenURL(
                "https://docs.google.com/document/d/1o_wtpxailmEGdtEwp5BGIyV8SXklvlJQp9vY2YoTBx4/edit?usp=sharing");

        private void UpdateSceneViewEvent()
        {
            Event e = Event.current;

            HandleKeyboardEvents(e);
            SceneViewEventHandler.HandleSceneViewEvent(e);
        }

        private void OnMainGUI()
        {
            GUILayout.Space(5);

            if (!ToolsWindow.IsOpen)
            {
                WindowData.Instance.WindowToolStackEditor.OnTabStackGUI();
            }

            WindowData.Instance.WindowToolStackEditor.DrawSelectedSettings();
            WindowData.Instance.Save();
            GlobalSettings.Instance.Save();
        }
    }
}
#endif
