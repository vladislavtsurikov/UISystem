using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using VladislavTsurikov.UIToolkitUtility.Editor.ElementStack.TabStack;

namespace VladislavTsurikov.ToolSystem.Editor.UIToolkit
{
    public class ToolGroupTabBar
    {
        private readonly List<ToolGroupTab> _groups;
        private readonly TabStackEditor<ToolGroupTab> _tabStack;
        private VisualElement _root;

        public event Action<string> GroupSelected;
        public string SelectedGroup { get; private set; }

        public ToolGroupTabBar(IEnumerable<string> groupNames, string defaultGroup = "Default")
        {
            _groups = groupNames.Select(name => new ToolGroupTab(name)).ToList();
            if (_groups.Count == 0)
            {
                _groups.Add(new ToolGroupTab(defaultGroup));
            }

            SelectedGroup = _groups.Any(group => group.Name == defaultGroup)
                ? defaultGroup
                : _groups[0].Name;

            foreach (var group in _groups)
            {
                group.Selected = group.Name == SelectedGroup;
            }

            _tabStack = new TabStackEditor<ToolGroupTab>(_groups)
            {
                TabName = tab => tab.Name,
                SelectCallback = SelectGroup,
                IsSelected = tab => tab.Selected,
                Draggable = false,
                EnableRename = false,
                TabWidthFromName = true
            };
        }

        public VisualElement CreateVisualElement()
        {
            _root = new VisualElement();
            _root.style.flexDirection = FlexDirection.Column;

            var tabElement = _tabStack.CreateGUI();
            _root.Add(tabElement);

            return _root;
        }

        private void SelectGroup(int index)
        {
            if (index < 0 || index >= _groups.Count)
            {
                return;
            }

            foreach (var group in _groups)
            {
                group.Selected = false;
            }

            _groups[index].Selected = true;
            SelectedGroup = _groups[index].Name;
            _tabStack.Refresh();
            GroupSelected?.Invoke(SelectedGroup);
        }

        private class ToolGroupTab
        {
            public ToolGroupTab(string name)
            {
                Name = name;
            }

            public string Name { get; }
            public bool Selected { get; set; }
        }
    }
}
