#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem;
using GlobalSettings_ElementsSystem_ToolsComponentStack =
    VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem.ToolsComponentStack;

namespace VladislavTsurikov.MegaWorld.Editor.Core.Window.ElementSystem
{
    public class ToolsComponentStackEditor : IMGUIComponentStackEditor<ToolComponentStack, ToolStackElementEditor>
    {
        private readonly GlobalSettings_ElementsSystem_ToolsComponentStack _toolsComponentStack;

        public ToolsComponentStackEditor(GlobalSettings_ElementsSystem_ToolsComponentStack stack) : base(stack) =>
            _toolsComponentStack = stack;

        public static void OnGUI(Type toolType, Type type)
        {
            foreach (ToolStackElementEditor item in GlobalSettings.Instance.ToolsComponentStackEditor.Editors)
            {
                var toolComponentStack = (ToolComponentStack)item.Target;

                if (toolComponentStack == null)
                {
                    continue;
                }

                if (toolComponentStack.ToolType == toolType)
                {
                    item.GeneralComponentStackEditor.DrawElement(type);
                }
            }
        }

        public static IMGUIElementEditor GetEditor(Type toolType, Type type)
        {
            foreach (ToolStackElementEditor item in GlobalSettings.Instance.ToolsComponentStackEditor.Editors)
            {
                var toolComponentStack = (ToolComponentStack)item.Target;

                if (toolComponentStack.ToolType == toolType)
                {
                    return item.GeneralComponentStackEditor.GetEditor(type);
                }
            }

            return null;
        }

        public void ResetStackMenu(Type toolType)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Reset"), false, () => _toolsComponentStack.Reset(toolType));

            menu.ShowAsContext();
        }
    }
}
#endif
