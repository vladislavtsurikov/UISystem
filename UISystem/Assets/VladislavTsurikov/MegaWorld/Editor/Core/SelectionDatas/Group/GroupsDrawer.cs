#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.IMGUIUtility.Runtime.ElementStack.IconStack;
using VladislavTsurikov.MegaWorld.Runtime.Core;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Utility;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Editor;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group
{
    public abstract class GroupsDrawer
    {
        private readonly List<ClipboardGroup> _clipboardGroups = new();

        private readonly Type _toolType;

        protected readonly SelectionData Data;

        protected GroupsDrawer(SelectionData data, Type toolType)
        {
            Data = data;
            _toolType = toolType;

            foreach (Type type in AllPrototypeTypes.TypeList)
            {
                _clipboardGroups.Add(new ClipboardGroup(toolType, type));
            }
        }

        public abstract void OnGUI();

        protected void GroupWindowMenu()
        {
            var menu = new GenericMenu();

            foreach (Type type in ToolUtility.GetSupportedPrototypeTypes(_toolType))
            {
                menu.AddItem(new GUIContent("Create Group/" + type.GetAttribute<NameAttribute>().Name), false,
                    ContextMenuUtility.ContextMenuCallback, new Action(() => CreateGroup(Data.GroupList, type)));
            }

            List<Runtime.Core.SelectionDatas.Group.Group> groups = AllAvailableGroups.GetAllGroups();

            if (groups.Count != 0)
            {
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Show All Group"), false, ContextMenuUtility.ContextMenuCallback,
                    new Action(() => EditorGUIUtility.PingObject(groups[0])));
            }

            menu.ShowAsContext();
        }

        protected GenericMenu GroupMenu(IShowIcon icon)
        {
            var menu = new GenericMenu();

            var group = (Runtime.Core.SelectionDatas.Group.Group)icon;

            menu.AddItem(new GUIContent("Reveal in Project"), false, ContextMenuUtility.ContextMenuCallback,
                new Action(() => EditorGUIUtility.PingObject(group)));
            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Delete"), false, ContextMenuUtility.ContextMenuCallback,
                new Action(() => DeleteSelectedGroups(Data.GroupList)));

            Type prototypeType = GroupUtility.GetGeneralPrototypeType(Data.SelectedData.SelectedGroupList);

            if (prototypeType != null)
            {
                ClipboardGroup clipboardGroup =
                    ClipboardObject.GetCurrentClipboardObject(prototypeType, _toolType, _clipboardGroups);
                clipboardGroup.GroupMenu(menu, Data.SelectedData);
            }

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Rename"), group.Renaming, ContextMenuUtility.ContextMenuCallback,
                new Action(() => group.Renaming = !group.Renaming));

            return menu;
        }

        private static void CreateGroup(List<Runtime.Core.SelectionDatas.Group.Group> groupList, Type prototypeType)
        {
            Runtime.Core.SelectionDatas.Group.Group newGroup = GroupUtility.CreateGroup(prototypeType);
            groupList.Add(newGroup);
        }

        private static void DeleteSelectedGroups(List<Runtime.Core.SelectionDatas.Group.Group> groupList) =>
            groupList.RemoveAll(group => group.Selected);
    }
}
#endif
