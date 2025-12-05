#if UNITY_EDITOR
using System;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;

namespace VladislavTsurikov.MegaWorld.Editor.Core.Window
{
    public class SelectionDataDrawer
    {
        public GroupsDrawer GroupsDrawer;
        public PrototypesDrawer PrototypesDrawer;

        public SelectionDataDrawer(Type selectionGroupWindowType, Type selectionPrototypeWindowType,
            SelectionData selectionData, Type toolType)
        {
            if (selectionPrototypeWindowType != null)
            {
                PrototypesDrawer =
                    (PrototypesDrawer)Activator.CreateInstance(selectionPrototypeWindowType, selectionData, toolType);
            }
            else
            {
                PrototypesDrawer = new IconPrototypesDrawer(selectionData, toolType);
            }

            if (selectionGroupWindowType != null)
            {
                GroupsDrawer =
                    (GroupsDrawer)Activator.CreateInstance(selectionGroupWindowType, selectionData, toolType);
            }
            else
            {
                GroupsDrawer = new IconGroupsDrawer(selectionData, toolType);
            }
        }

        public virtual void OnGUI(SelectionData selectionData, Type toolType)
        {
            GroupsDrawer?.OnGUI();
            PrototypesDrawer?.OnGUI();
        }
    }
}
#endif
