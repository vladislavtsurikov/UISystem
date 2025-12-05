#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Utility;

namespace VladislavTsurikov.MegaWorld.Editor.Core.Window
{
    public partial class MegaWorldWindow
    {
        private void HandleKeyboardEvents(Event current)
        {
            if (current.Equals(KeyDeleteEvent()))
            {
                UnspawnUtility.UnspawnGroups(WindowData.Instance.SelectedData.SelectedGroupList, true);
            }

            if (current.keyCode == KeyCode.Escape && current.modifiers == 0)
            {
                if (WindowData.Instance.SelectedTool != null)
                {
                    WindowData.Instance.WindowToolStack.OnDisable();
                    Tools.current = Tool.Move;
                }

                Repaint();
            }
        }

        private static Event KeyDeleteEvent()
        {
            var retEvent = Event.KeyboardEvent("^" + "backspace");
            return retEvent;
        }
    }
}
#endif
