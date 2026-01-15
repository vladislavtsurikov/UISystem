using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.Nody.Editor.Core;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ToolSystem.Editor.Core;
using VladislavTsurikov.ToolSystem.Runtime.Core;
using VladislavTsurikov.UIToolkitUtility.Editor.ElementStack.TabStack;

namespace VladislavTsurikov.ToolSystem.Editor.UIToolkit
{
    public class ToolStackEditor
    {
        private readonly ToolStack _toolStack;
        private VisualElement _root;
        private VisualElement _selectedToolContent;
        private ScrollView _contentScrollView;

        private Dictionary<string, List<Type>> _toolsByGroup;
        private string _selectedGroup = "Default";
        private readonly List<EditorTool> _visibleTools = new List<EditorTool>();
        private readonly List<UIToolkitToolEditor> _toolEditors = new List<UIToolkitToolEditor>();
        private ToolGroupTabBar _groupTabBar;
        private TabStackEditor<EditorTool> _toolTabStack;
        private VisualElement _toolTabsRoot;

        public ToolStackEditor(ToolStack toolStack)
        {
            _toolStack = toolStack;
            _toolsByGroup = ToolManager.GetToolsByGroup();
            InitializeEditors();
        }

        private void InitializeEditors()
        {
            _toolEditors.Clear();
            foreach (var tool in _toolStack.ElementList)
            {
                var editorType = AllEditorTypes<EditorTool>.Get(tool.GetType());
                if (editorType != null && typeof(UIToolkitToolEditor).IsAssignableFrom(editorType))
                {
                    var editor = (UIToolkitToolEditor)Activator.CreateInstance(editorType);
                    editor.Init(tool);
                    _toolEditors.Add(editor);
                }
            }
        }

        public VisualElement CreateVisualElement()
        {
            _root = new VisualElement();
            _root.style.flexDirection = FlexDirection.Column;
            _root.style.flexGrow = 1;

            CreateGroupTabs();
            CreateToolTabs();
            CreateContentArea();

            RefreshToolTabs();

            return _root;
        }

        private void CreateGroupTabs()
        {
            _groupTabBar = new ToolGroupTabBar(_toolsByGroup.Keys.OrderBy(g => g), _selectedGroup);
            _groupTabBar.GroupSelected += SelectGroup;
            var groupTabs = _groupTabBar.CreateVisualElement();
            groupTabs.style.backgroundColor = new Color(0.25f, 0.25f, 0.25f);
            groupTabs.style.paddingTop = 5;
            groupTabs.style.paddingBottom = 5;
            groupTabs.style.paddingLeft = 5;
            groupTabs.style.paddingRight = 5;
            groupTabs.style.marginBottom = 5;
            _root.Add(groupTabs);
        }

        private void CreateToolTabs()
        {
            _toolTabsRoot = new VisualElement();
            _toolTabsRoot.style.backgroundColor = new Color(0.22f, 0.22f, 0.22f);
            _toolTabsRoot.style.paddingTop = 5;
            _toolTabsRoot.style.paddingBottom = 5;
            _toolTabsRoot.style.paddingLeft = 5;
            _toolTabsRoot.style.paddingRight = 5;
            _toolTabsRoot.style.marginBottom = 5;
            _toolTabsRoot.style.flexDirection = FlexDirection.Column;

            _toolTabStack = new TabStackEditor<EditorTool>(_visibleTools)
            {
                AddCallback = ShowAddToolMenu,
                EnableRename = true,
                Draggable = true,
                MoveCallback = SyncToolOrder,
                RenameCallback = RenameTool,
                SelectCallback = SelectTool,
                TabMenuCallback = CreateToolContextMenu,
                TabName = GetToolTabName,
                IsSelected = tool => tool.Selected
            };

            _toolTabsRoot.Add(_toolTabStack.CreateGUI());
            _root.Add(_toolTabsRoot);
        }

        private void SelectGroup(string groupName)
        {
            _selectedGroup = groupName;
            RefreshToolTabs();
        }

        private void CreateContentArea()
        {
            var contentArea = new VisualElement();
            contentArea.style.flexDirection = FlexDirection.Column;
            contentArea.style.flexGrow = 1;
            contentArea.style.backgroundColor = new Color(0.24f, 0.24f, 0.24f);
            contentArea.style.paddingTop = 10;
            contentArea.style.paddingLeft = 10;
            contentArea.style.paddingRight = 10;

            _contentScrollView = new ScrollView();
            _contentScrollView.style.flexGrow = 1;
            _selectedToolContent = new VisualElement();
            _contentScrollView.Add(_selectedToolContent);
            contentArea.Add(_contentScrollView);

            _root.Add(contentArea);
        }

        private void RefreshToolTabs()
        {
            _visibleTools.Clear();
            var toolsInStack = _toolStack.ElementList
                .Where(tool => ToolManager.GetToolGroup(tool.GetType()) == _selectedGroup)
                .ToList();

            _visibleTools.AddRange(toolsInStack);

            if (_toolTabStack != null)
            {
                _toolTabStack.Refresh();
            }

            var selectedTool = _toolStack.ElementList.FirstOrDefault(tool => tool.Selected);
            if (selectedTool == null || ToolManager.GetToolGroup(selectedTool.GetType()) != _selectedGroup)
            {
                selectedTool = _visibleTools.FirstOrDefault();
                if (selectedTool != null)
                {
                    SelectTool(_visibleTools.IndexOf(selectedTool));
                }
                else
                {
                    _selectedToolContent.Clear();
                }
            }
            else
            {
                RefreshSelectedToolContent(selectedTool);
            }
        }

        private string GetToolTabName(EditorTool tool)
        {
            if (tool is IHasName namedTool && !string.IsNullOrEmpty(namedTool.Name))
            {
                return namedTool.Name;
            }

            return ToolManager.GetToolName(tool.GetType());
        }

        private GenericMenu CreateToolContextMenu(int index)
        {
            var menu = new GenericMenu();
            if (index < 0 || index >= _visibleTools.Count)
            {
                return menu;
            }

            var tool = _visibleTools[index];
            menu.AddItem(new GUIContent("Delete"), false, () => DeleteTool(tool));
            return menu;
        }

        private void ShowAddToolMenu()
        {
            var menu = new GenericMenu();

            if (!_toolsByGroup.ContainsKey(_selectedGroup))
            {
                menu.AddDisabledItem(new GUIContent("No tools available"));
                menu.ShowAsContext();
                return;
            }

            var availableTools = _toolsByGroup[_selectedGroup];

            foreach (var toolType in availableTools)
            {
                if (toolType.IsAbstract)
                    continue;

                var toolName = ToolManager.GetToolName(toolType);
                var hasType = _toolStack.HasType(toolType);

                if (!hasType)
                {
                    menu.AddItem(new GUIContent(toolName), false, () => AddTool(toolType));
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent(toolName + " (already added)"));
                }
            }

            menu.ShowAsContext();
        }

        private void AddTool(Type toolType)
        {
            var tool = _toolStack.CreateIfMissingType(toolType);
            if (tool != null)
            {
                // Create editor for the new tool
                var editorType = AllEditorTypes<EditorTool>.Get(tool.GetType());
                if (editorType != null && typeof(UIToolkitToolEditor).IsAssignableFrom(editorType))
                {
                    var editor = (UIToolkitToolEditor)Activator.CreateInstance(editorType);
                    editor.Init(tool);
                    _toolEditors.Add(editor);
                }

                RefreshToolTabs();
                SelectTool(_visibleTools.IndexOf(tool));
            }
        }

        private void SelectTool(int index)
        {
            if (index < 0 || index >= _visibleTools.Count)
            {
                return;
            }

            foreach (var tool in _toolStack.ElementList)
            {
                tool.Selected = false;
            }

            var selectedTool = _visibleTools[index];
            selectedTool.Selected = true;
            RefreshToolTabs();
            RefreshSelectedToolContent(selectedTool);
        }

        private void RefreshSelectedToolContent(EditorTool tool)
        {
            _selectedToolContent.Clear();

            var title = new Label(ToolManager.GetToolName(tool.GetType()));
            title.style.fontSize = 18;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.marginBottom = 10;
            _selectedToolContent.Add(title);

            var description = ToolManager.GetToolDescription(tool.GetType());
            if (!string.IsNullOrEmpty(description))
            {
                var descLabel = new Label(description);
                descLabel.style.fontSize = 12;
                descLabel.style.color = new Color(0.7f, 0.7f, 0.7f);
                descLabel.style.marginBottom = 15;
                _selectedToolContent.Add(descLabel);
            }

            var docUrl = ToolManager.GetToolDocumentation(tool.GetType());
            if (!string.IsNullOrEmpty(docUrl))
            {
                var docButton = new Button(() => Application.OpenURL(docUrl));
                docButton.text = "Open Documentation";
                docButton.style.marginBottom = 15;
                _selectedToolContent.Add(docButton);
            }

            var editor = _toolEditors.FirstOrDefault(e => e.Target == tool);
            if (editor != null)
            {
                var toolUI = editor.CreateGUI();
                if (toolUI != null)
                {
                    _selectedToolContent.Add(toolUI);
                }
            }
        }

        private void DeleteTool(EditorTool tool)
        {
            if (EditorUtility.DisplayDialog("Delete Tool",
                $"Are you sure you want to delete '{ToolManager.GetToolName(tool.GetType())}'?",
                "Delete", "Cancel"))
            {
                var index = _toolStack.ElementList.IndexOf(tool);
                if (index >= 0)
                {
                    var editor = _toolEditors.FirstOrDefault(e => e.Target == tool);
                    if (editor != null)
                    {
                        _toolEditors.Remove(editor);
                    }

                    _toolStack.Remove(index);
                    RefreshToolTabs();
                    _selectedToolContent.Clear();
                }
            }
        }

        private void RenameTool(int index, string newName)
        {
            if (index < 0 || index >= _visibleTools.Count)
            {
                return;
            }

            if (_visibleTools[index] is IHasName namedTool)
            {
                namedTool.Name = newName;
            }
        }

        private void SyncToolOrder()
        {
            var ordered = _toolStack.ElementList.Where(tool =>
                ToolManager.GetToolGroup(tool.GetType()) == _selectedGroup).ToList();

            if (ordered.Count != _visibleTools.Count)
            {
                return;
            }

            var currentIndex = 0;
            for (var i = 0; i < _toolStack.ElementList.Count; i++)
            {
                if (ToolManager.GetToolGroup(_toolStack.ElementList[i].GetType()) != _selectedGroup)
                {
                    continue;
                }

                _toolStack.ElementList[i] = _visibleTools[currentIndex];
                currentIndex++;
            }
        }
    }
}
