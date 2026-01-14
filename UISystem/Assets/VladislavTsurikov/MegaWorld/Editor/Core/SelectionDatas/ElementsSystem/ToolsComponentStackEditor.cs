#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Editor.Core.Window.ElementSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.ElementsSystem
{
    public class ToolsComponentStackEditor : IMGUIComponentStackEditor<ToolComponentStack, ToolStackElementEditor>
    {
        public ToolsComponentStackEditor(AdvancedNodeStack<ToolComponentStack> list) : base(list)
        {
        }

        public void DrawElements(Type toolType, List<Type> types)
        {
            foreach (ToolStackElementEditor item in Editors)
            {
                var toolComponentStack = (ToolComponentStack)item.Target;

                if (toolComponentStack == null)
                {
                    continue;
                }

                if (toolComponentStack.ToolType == toolType)
                {
                    item.GeneralComponentStackEditor.DrawElements(types);
                }
            }
        }
    }
}
#endif
