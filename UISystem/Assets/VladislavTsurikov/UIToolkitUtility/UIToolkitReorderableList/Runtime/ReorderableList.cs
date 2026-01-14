#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace VladislavTsurikov.UIToolkitReorderableList.Runtime
{
    public class ReorderableList : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<ReorderableList, UxmlTraits> { }

        private readonly ListView _listView;
        private readonly VisualElement _header;
        private readonly VisualElement _footer;
        private readonly Label _headerLabel;
        private readonly Button _addButton;
        private readonly Button _removeButton;

        private IList _itemsSource;
        private Type _itemType;

        public string HeaderTitle
        {
            get => _headerLabel.text;
            set => _headerLabel.text = value;
        }

        public bool ShowHeader { get; set; } = true;
        public bool ShowFooter { get; set; } = true;
        public bool ShowAddButton { get; set; } = true;
        public bool ShowRemoveButton { get; set; } = true;
        public bool Reorderable { get; set; } = true;

        public Func<VisualElement> MakeItem { get; set; }
        public Action<VisualElement, int> BindItem { get; set; }
        public Func<int, float> GetItemHeight { get; set; }
        public Action<IList> OnAdd { get; set; }
        public Action<int> OnRemove { get; set; }
        public Action OnReorder { get; set; }

        public int SelectedIndex
        {
            get => _listView.selectedIndex;
            set => _listView.selectedIndex = value;
        }

        public IList ItemsSource
        {
            get => _itemsSource;
            set
            {
                _itemsSource = value;
                _listView.itemsSource = value;
                _listView.Rebuild();
            }
        }

        public ReorderableList()
        {
            AddToClassList("reorderable-list");

            // Header
            _header = new VisualElement();
            _header.AddToClassList("reorderable-list__header");
            _header.style.flexDirection = FlexDirection.Row;
            _header.style.justifyContent = Justify.SpaceBetween;
            _header.style.alignItems = Align.Center;
            _header.style.backgroundColor = new Color(0.22f, 0.22f, 0.22f, 1f);
            _header.style.paddingLeft = 6;
            _header.style.paddingRight = 6;
            _header.style.paddingTop = 2;
            _header.style.paddingBottom = 2;
            _header.style.minHeight = 20;

            _headerLabel = new Label("List");
            _headerLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            _headerLabel.style.fontSize = 12;
            _header.Add(_headerLabel);

            Add(_header);

            // ListView
            _listView = new ListView
            {
                reorderable = true,
                reorderMode = ListViewReorderMode.Animated,
                showBorder = true,
                showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly,
                selectionType = SelectionType.Single,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight
            };

            _listView.AddToClassList("reorderable-list__list-view");
            _listView.style.flexGrow = 1;
            _listView.style.minHeight = 50;

            Add(_listView);

            // Footer
            _footer = new VisualElement();
            _footer.AddToClassList("reorderable-list__footer");
            _footer.style.flexDirection = FlexDirection.Row;
            _footer.style.justifyContent = Justify.FlexEnd;
            _footer.style.backgroundColor = new Color(0.22f, 0.22f, 0.22f, 1f);
            _footer.style.paddingLeft = 4;
            _footer.style.paddingRight = 4;
            _footer.style.paddingTop = 2;
            _footer.style.paddingBottom = 2;
            _footer.style.minHeight = 18;

            _addButton = new Button(() => HandleAdd())
            {
                text = "+"
            };
            _addButton.style.width = 25;
            _addButton.style.height = 16;
            _addButton.style.fontSize = 12;
            _addButton.style.unityFontStyleAndWeight = FontStyle.Bold;

            _removeButton = new Button(() => HandleRemove())
            {
                text = "-"
            };
            _removeButton.style.width = 25;
            _removeButton.style.height = 16;
            _removeButton.style.fontSize = 12;
            _removeButton.style.unityFontStyleAndWeight = FontStyle.Bold;
            _removeButton.style.marginLeft = 2;

            _footer.Add(_addButton);
            _footer.Add(_removeButton);

            Add(_footer);

            // Setup ListView callbacks
            _listView.makeItem = () => CreateListItem();
            _listView.bindItem = (element, index) => BindListItem(element, index);
            _listView.itemsAdded += OnItemsAdded;
            _listView.itemsRemoved += OnItemsRemoved;
            _listView.itemIndexChanged += OnItemIndexChanged;

            UpdateVisibility();
        }

        private VisualElement CreateListItem()
        {
            var listElement = new ReorderableListElement();

            if (MakeItem != null)
            {
                var customContent = MakeItem.Invoke();
                listElement.ContentContainer.Add(customContent);
            }
            else
            {
                listElement.ContentContainer.Add(CreateDefaultItem());
            }

            listElement.SetDragHandleVisible(Reorderable);
            return listElement;
        }

        private void BindListItem(VisualElement element, int index)
        {
            if (element is ReorderableListElement listElement)
            {
                VisualElement content = listElement.ContentContainer.childCount > 0
                    ? listElement.ContentContainer[0]
                    : null;

                if (content != null)
                {
                    BindItem?.Invoke(content, index);
                }
            }
        }

        public void Refresh()
        {
            _listView.Rebuild();
        }

        private VisualElement CreateDefaultItem()
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.paddingLeft = 4;
            container.style.paddingRight = 4;
            container.style.paddingTop = 2;
            container.style.paddingBottom = 2;
            container.Add(new Label("Item"));
            return container;
        }

        private void HandleAdd()
        {
            if (_itemsSource == null)
            {
                return;
            }

            if (OnAdd != null)
            {
                OnAdd.Invoke(_itemsSource);
            }
            else
            {
                object newItem = CreateDefaultInstance(_itemType);
                _itemsSource.Add(newItem);
            }

            _listView.Rebuild();
        }

        private void HandleRemove()
        {
            int selectedIndex = _listView.selectedIndex;
            if (selectedIndex < 0 || selectedIndex >= _itemsSource?.Count)
            {
                return;
            }

            if (OnRemove != null)
            {
                OnRemove.Invoke(selectedIndex);
            }
            else
            {
                _itemsSource.RemoveAt(selectedIndex);
            }

            _listView.Rebuild();
        }

        private void OnItemsAdded(IEnumerable<int> indices)
        {
            // Items were added through drag and drop or other means
        }

        private void OnItemsRemoved(IEnumerable<int> indices)
        {
            // Items were removed
        }

        private void OnItemIndexChanged(int oldIndex, int newIndex)
        {
            OnReorder?.Invoke();
        }

        private void UpdateVisibility()
        {
            _header.style.display = ShowHeader ? DisplayStyle.Flex : DisplayStyle.None;
            _footer.style.display = ShowFooter ? DisplayStyle.Flex : DisplayStyle.None;
            _addButton.style.display = ShowAddButton ? DisplayStyle.Flex : DisplayStyle.None;
            _removeButton.style.display = ShowRemoveButton ? DisplayStyle.Flex : DisplayStyle.None;
            _listView.reorderable = Reorderable;
        }

        public void SetItemType(Type itemType)
        {
            _itemType = itemType;
        }

        private static object CreateDefaultInstance(Type type)
        {
            if (type == null)
            {
                return null;
            }

            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                return null;
            }

            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            if (type == typeof(string))
            {
                return string.Empty;
            }

            if (type.GetConstructor(Type.EmptyTypes) != null)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }
    }
}
#endif
