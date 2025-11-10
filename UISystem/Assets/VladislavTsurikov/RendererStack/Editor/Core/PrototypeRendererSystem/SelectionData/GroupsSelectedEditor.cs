#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.RendererStack.Runtime.Core;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem;
using VladislavTsurikov.UnityUtility.Editor;

namespace VladislavTsurikov.RendererStack.Editor.Core.PrototypeRendererSystem.SelectionData
{
    public class GroupsSelectedEditor
    {
        private readonly Runtime.Core.PrototypeRendererSystem.SelectionData _selectionData;
        private readonly TabStackEditor _tabStackEditor;

        public GroupsSelectedEditor(Runtime.Core.PrototypeRendererSystem.SelectionData selectionData)
        {
            _selectionData = selectionData;

            _tabStackEditor = new TabStackEditor(selectionData.GroupList, true, true)
            {
                TabHeight = 22,
                TabWidthFromName = false,
                AddCallback = AddGroup,
                AddTabMenuCallback = TabMenu,
                SelectCallback = SelectGroup,
                HappenedMoveCallback = HappenedMoveCallback
            };
        }

        public void OnGUI() => _tabStackEditor.OnGUI();

        private void AddGroup() => SelectGroup(AddGroup("New Group"));

        private void SelectGroup(int index)
        {
            if (index < 0 && index >= _selectionData.GroupList.Count)
            {
                return;
            }

            foreach (Group tab in _selectionData.GroupList)
            {
                tab.Selected = false;
            }

            DisableAllPrototypes();
            _selectionData.GroupList[index].SelectAllPrototypes();
            _selectionData.GroupList[index].Selected = true;
        }

        private void DisableAllPrototypes() => _selectionData.PrototypeList.ForEach(proto => proto.Selected = false);

        private int AddGroup(string name)
        {
            _selectionData.GroupList.Add(new Group(name));
            return _selectionData.GroupList.Count - 1;
        }

        private void DeleteSelectedGroups()
        {
            foreach (Group group in _selectionData.GroupList)
            {
                if (group.Selected)
                {
                    _selectionData.RemovePrototypes(group.PrototypeList);
                }
            }

            _selectionData.GroupList.RemoveAll(group => group.Selected);

            Group lastGroup = _selectionData.GroupList.Last();

            if (lastGroup != null)
            {
                lastGroup.Selected = true;
            }
        }

        private void HappenedMoveCallback() => RendererStackManager.Instance.Setup(true);

        private GenericMenu TabMenu(int currentTabIndex)
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("Add Tab"), false, ContextMenuUtility.ContextMenuCallback,
                new Action(AddGroup));

            menu.AddSeparator("");
            if (_selectionData.GroupList.Count > 1)
            {
                menu.AddItem(new GUIContent("Delete"), false, ContextMenuUtility.ContextMenuCallback,
                    new Action(DeleteSelectedGroups));
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Delete"));
            }

            return menu;
        }
    }
}
#endif
