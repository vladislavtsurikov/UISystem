#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.DeepCopy.Runtime;
using VladislavTsurikov.Nody.Editor.Core;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.ReflectionUtility.Runtime;
using ReorderableList = VladislavTsurikov.UIToolkitReorderableList.Runtime.ReorderableList;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack.UIToolkitReorderableList
{
    public class ReorderableListStackEditor<T, N> : NodeStackEditor<T, N>
        where T : Node
        where N : UIToolkitReorderableListComponentEditor
    {
        private AdvancedNodeStack<T> _advancedNodeStack;

        private ReorderableList _reorderableList;
        private readonly GUIContent _listName;
        private Node _copyComponentElement;

        protected bool CopySettings = true;

        public bool DisplayHeaderText = true;
        public bool DisplayPlusButton = true;
        public bool DuplicateSupport = true;
        public bool RenameSupport = false;
        public bool ShowActiveToggle = true;
        public bool RemoveSupport = true;
        public bool ReorderSupport = true;

        public ReorderableListStackEditor(AdvancedNodeStack<T> stack) : base(stack)
        {
            _advancedNodeStack = stack;
            _listName = new GUIContent("");
            InitializeReorderableList();
        }

        public ReorderableListStackEditor(GUIContent listName, AdvancedNodeStack<T> stack,
            bool displayHeader) : base(stack)
        {
            _listName = listName;
            InitializeReorderableList();
        }

        private void InitializeReorderableList()
        {
            _reorderableList = new ReorderableList
            {
                HeaderTitle = _listName?.text ?? "Components",
                ShowHeader = DisplayHeaderText,
                ShowFooter = DisplayPlusButton,
                ShowAddButton = DisplayPlusButton,
                ShowRemoveButton = true,
                Reorderable = ReorderSupport
            };

            _reorderableList.SetItemType(typeof(T));
            _reorderableList.MakeItem = MakeListItem;
            _reorderableList.BindItem = BindListItem;
            _reorderableList.OnAdd = OnAdd;
            _reorderableList.OnRemove = OnRemove;
            _reorderableList.OnReorder = OnReorder;

            RefreshList();
        }

        public VisualElement GetVisualElement()
        {
            if (Stack.IsDirty)
            {
                Stack.RemoveInvalidElements();
                RefreshEditors();
                RefreshList();
                Stack.IsDirty = false;
            }

            return _reorderableList;
        }

        private VisualElement MakeListItem()
        {
            var element = new ComponentListElement();

            element.OnMenuClicked = HandleMenuClick;
            element.OnSelectionChanged = HandleSelection;
            element.OnActiveChanged = HandleActiveChanged;
            element.OnRenameComplete = HandleRenameComplete;
            element.SetDragHandleVisible(ReorderSupport);

            return element;
        }

        private void BindListItem(VisualElement visualElement, int index)
        {
            if (visualElement is not ComponentListElement componentElement)
                return;

            if (index < 0 || index >= Stack.ElementList.Count || index >= Editors.Count)
                return;

            var component = Stack.ElementList[index];
            var editor = Editors[index];

            if (component == null || editor == null)
                return;

            componentElement.SetComponent(component, ShowActiveToggle);
            componentElement.IsSelected = _reorderableList.SelectedIndex == index;

            // Create content for foldout
            var content = editor.CreateGUI();
            componentElement.SetContent(content);
        }

        private void HandleMenuClick(ComponentListElement element)
        {
            int index = GetElementIndex(element);
            if (index >= 0 && index < Stack.ElementList.Count)
            {
                ShowContextMenu(Stack.ElementList[index], index);
            }
        }

        private void HandleSelection(ComponentListElement element)
        {
            int index = GetElementIndex(element);
            if (index >= 0)
            {
                _reorderableList.SelectedIndex = index;
                Stack.Select(Stack.ElementList[index]);
                _reorderableList.Refresh();
            }
        }

        private void HandleActiveChanged(ComponentListElement element, bool active)
        {
            int index = GetElementIndex(element);
            if (index >= 0 && index < Stack.ElementList.Count)
            {
                Stack.ElementList[index].Active = active;
            }
        }

        private void HandleRenameComplete(ComponentListElement element, string newName)
        {
            int index = GetElementIndex(element);
            if (index >= 0 && index < Stack.ElementList.Count)
            {
                Stack.ElementList[index].Name = newName;
                Stack.ElementList[index].Renaming = false;
                _reorderableList.Refresh();
            }
        }

        private int GetElementIndex(ComponentListElement element)
        {
            var parent = element.parent;
            if (parent == null)
                return -1;

            for (int i = 0; i < parent.childCount; i++)
            {
                if (parent[i] == element)
                    return i;
            }

            return -1;
        }

        private void OnAdd(System.Collections.IList list)
        {
            ShowAddMenu();
        }

        private void OnRemove(int index)
        {
            if (index >= 0 && index < Stack.ElementList.Count)
            {
                var component = Stack.ElementList[index];
                if (component.IsDeletable())
                {
                    Stack.Remove(index);
                    RefreshEditors();
                    RefreshList();
                }
            }
        }

        private void OnReorder()
        {
            Stack.IsDirty = true;
        }

        private void RefreshList()
        {
            _reorderableList.ItemsSource = _advancedNodeStack.List;
            _reorderableList.Refresh();
        }

        protected virtual void ShowAddMenu()
        {
            var menu = new GenericMenu();

            foreach (Type componentType in GetComponentTypes())
            {
                if (componentType.GetAttribute(typeof(DontCreateAttribute)) != null)
                    continue;

                if (componentType.GetAttribute<PersistentNodeAttribute>() != null ||
                    componentType.GetAttribute<DontShowInAddMenuAttribute>() != null)
                    continue;

                var nameAttr = componentType.GetAttribute<NameAttribute>();
                var context = nameAttr?.Name ?? componentType.Name;

                if (Stack is NodeStackSupportSameType<T> componentStackWithSameTypes)
                {
                    menu.AddItem(new GUIContent(context), false,
                        () =>
                        {
                            componentStackWithSameTypes.CreateNode(componentType);
                            RefreshEditors();
                            RefreshList();
                        });
                }
                else if (Stack is NodeStackOnlyDifferentTypes<T> componentStackWithDifferentTypes)
                {
                    var exists = componentStackWithDifferentTypes.HasType(componentType);

                    if (!exists)
                    {
                        menu.AddItem(new GUIContent(context), false,
                            () =>
                            {
                                componentStackWithDifferentTypes.CreateIfMissingType(componentType);
                                RefreshEditors();
                                RefreshList();
                            });
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent(context));
                    }
                }
            }

            menu.ShowAsContext();
        }

        protected virtual void ShowContextMenu(T component, int index)
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("Reset"), false, () =>
            {
                Stack.Reset(index);
                RefreshEditors();
                RefreshList();
            });

            if (RemoveSupport && component.IsDeletable())
            {
                menu.AddItem(new GUIContent("Remove"), false, () =>
                {
                    Stack.Remove(index);
                    RefreshEditors();
                    RefreshList();
                });
            }

            if (DuplicateSupport)
            {
                menu.AddItem(new GUIContent("Duplicate"), false, () =>
                {
                    DuplicateComponent(component, index + 1);
                    RefreshEditors();
                    RefreshList();
                });
            }

            if (RenameSupport)
            {
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Rename"), component.Renaming, () =>
                {
                    RenameComponent(component);
                });
            }

            if (CopySettings)
            {
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Copy Settings"), false,
                    () => _copyComponentElement = DeepCopier.Copy(component));

                if (_copyComponentElement != null)
                {
                    menu.AddItem(new GUIContent("Paste Settings"), false,
                        () =>
                        {
                            Stack.ReplaceElement((T)DeepCopier.Copy(_copyComponentElement), index);
                            RefreshEditors();
                            RefreshList();
                        });
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Paste Settings"), false);
                }
            }

            menu.ShowAsContext();
        }

        private void DuplicateComponent(T component, int index)
        {
            if (Stack is NodeStackSupportSameType<T> componentStackWithSameTypes)
            {
                componentStackWithSameTypes.CreateNode(component.GetType(), index);
                Stack.ReplaceElement(DeepCopier.Copy(component), index);
            }
        }

        private void RenameComponent(Node component)
        {
            component.Renaming = !component.Renaming;
            component.RenamingName = component.Name;

            // Find the visual element for this component and start rename
            var listView = _reorderableList.Q<ListView>();
            if (listView != null)
            {
                int index = Stack.IndexOf((T)component);
                if (index >= 0)
                {
                    _reorderableList.Refresh();

                    // Delay to ensure the element is rendered
                    _reorderableList.schedule.Execute(() =>
                    {
                        var scrollView = listView.Q<ScrollView>();
                        if (scrollView?.contentContainer != null && index < scrollView.contentContainer.childCount)
                        {
                            var itemElement = scrollView.contentContainer[index];
                            var componentElement = itemElement.Q<ComponentListElement>();
                            componentElement?.StartRename();
                        }
                    }).ExecuteLater(50);
                }
            }
        }

        protected List<Type> GetComponentTypes() =>
            AllTypesDerivedFrom<T>.Types.OrderBy(x => x.FullName).ThenBy(x => x.Namespace?.Split('.')[^1])
                .ToList();
    }
}
#endif
