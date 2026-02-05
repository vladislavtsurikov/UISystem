using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace VladislavTsurikov.UIToolkitUtility.Editor.ElementStack.TabStack
{
    public class TabStackEditor<T> : VisualElement
    {
        public delegate bool IsSelectedCallbackDelegate(T item);
        public delegate void AddCallbackDelegate();
        public delegate void MoveCallbackDelegate();
        public delegate void RenameCallbackDelegate(int index, string newName);
        public delegate void SelectCallbackDelegate(int index);
        public delegate GenericMenu TabMenuCallbackDelegate(int index);
        public delegate void TabColorCallbackDelegate(T item, out Color barColor, out Color labelColor);
        public delegate string TabNameCallbackDelegate(T item);

        private const float DragIndicatorWidth = 2f;
        private const string LayoutPath =
            "Assets/VladislavTsurikov/UIToolkitUtility/Editor/ElementStack/TabStack/TabStackEditor.uxml";
        private const string StylePath =
            "Assets/VladislavTsurikov/UIToolkitUtility/Editor/ElementStack/TabStack/TabStackEditor.uss";
        private const string LayoutContainerName = "LayoutContainer";
        private const string DragIndicatorName = "DragIndicator";

        private readonly IList<T> _elements;
        private readonly List<Tab> _tabButtons = new();
        private VisualElement _root;
        private VisualElement _dragIndicator;
        private int _dragIndex = -1;
        private int _dragTargetIndex = -1;
        private bool _dragAfter;

        public AddCallbackDelegate AddCallback;
        public bool Draggable = true;
        public bool EnableRename;
        public IsSelectedCallbackDelegate IsSelected;
        public MoveCallbackDelegate MoveCallback;
        public RenameCallbackDelegate RenameCallback;
        public SelectCallbackDelegate SelectCallback;
        public TabColorCallbackDelegate TabColor;
        public TabMenuCallbackDelegate TabMenuCallback;
        public TabNameCallbackDelegate TabName;

        public int OffsetTabWidth = 30;
        public int TabHeight = 25;
        public int TabPlusWidth = 50;
        public int TabWidth = 130;
        public bool TabWidthFromName = true;
        public Color SelectedBackgroundColor = new(0.4f, 0.6f, 0.8f);

        public TabStackEditor(IList<T> elements)
        {
            _elements = elements;
            CreateRoot();

            _root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            _root.RegisterCallback<PointerUpEvent>(OnPointerUp);

            Refresh();
        }

        public VisualElement CreateGUI()
        {
            return this;
        }

        private void CreateRoot()
        {
            var layout = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(LayoutPath);
            if (layout == null)
            {
                _root = this;
                _root.style.flexDirection = FlexDirection.Row;
                _root.style.flexWrap = Wrap.Wrap;

                _dragIndicator = CreateFallbackDragIndicator();
                _root.Add(_dragIndicator);
                return;
            }

            var template = layout.CloneTree();
            Add(template);
            _root = template.Q<VisualElement>(LayoutContainerName) ?? template;

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(StylePath);
            if (styleSheet != null)
            {
                styleSheets.Add(styleSheet);
            }

            _dragIndicator = template.Q<VisualElement>(DragIndicatorName) ?? CreateFallbackDragIndicator();
            if (_dragIndicator.parent == null)
            {
                _root.Add(_dragIndicator);
            }
        }

        private static VisualElement CreateFallbackDragIndicator()
        {
            var dragIndicator = new VisualElement { name = DragIndicatorName };
            dragIndicator.style.position = Position.Absolute;
            dragIndicator.style.width = DragIndicatorWidth;
            dragIndicator.style.backgroundColor = Color.white;
            dragIndicator.style.display = DisplayStyle.None;
            return dragIndicator;
        }

        public void Refresh()
        {
            if (_root == null)
            {
                return;
            }

            _root.Clear();
            _root.Add(_dragIndicator);
            _tabButtons.Clear();

            for (var index = 0; index < _elements.Count; index++)
            {
                var tabButton = CreateTabButton(index);
                _tabButtons.Add(tabButton);
                _root.Add(tabButton);
            }

            if (AddCallback != null)
            {
                var addButton = new TabPlus();
                addButton.Clicked += () => AddCallback();
                ApplyTabSize(addButton, TabPlusWidth);
                _root.Add(addButton);
            }
        }

        private Tab CreateTabButton(int index)
        {
            var item = _elements[index];
            var tabButton = new Tab();
            tabButton.Clicked += () => Select(index);
            tabButton.Text = TabName?.Invoke(item) ?? item?.ToString() ?? "Tab";

            ApplyTabSize(tabButton, GetTabWidth(tabButton.Text));

            if (TabColor != null)
            {
                TabColor(item, out var barColor, out var labelColor);
                tabButton.SetBackgroundColor(barColor);
                tabButton.SetLabelColor(labelColor);
            }
            else if (IsSelected != null && IsSelected(item))
            {
                tabButton.SetBackgroundColor(SelectedBackgroundColor);
            }

            tabButton.RegisterCallback<ContextClickEvent>(evt =>
            {
                evt.StopPropagation();
                if (TabMenuCallback == null)
                {
                    return;
                }

                if (IsSelected != null && !IsSelected(item))
                {
                    Select(index);
                    return;
                }

                var menu = TabMenuCallback(index);
                if (EnableRename && RenameCallback != null)
                {
                    menu.AddItem(new GUIContent("Rename"), false, () => BeginRename(index));
                }
                menu.ShowAsContext();
            });

            tabButton.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (!Draggable || evt.button != 0)
                {
                    return;
                }

                _dragIndex = index;
                _dragTargetIndex = index;
                _dragAfter = false;
                tabButton.CapturePointer(evt.pointerId);
            });

            return tabButton;
        }

        private void ApplyTabSize(VisualElement tabButton, float width)
        {
            tabButton.style.height = TabHeight;
            tabButton.style.minHeight = TabHeight;
            tabButton.style.width = width;
            tabButton.style.minWidth = width;
        }

        private float GetTabWidth(string text)
        {
            if (!TabWidthFromName)
            {
                return TabWidth;
            }

            var size = EditorStyles.label.CalcSize(new GUIContent(text)).x + OffsetTabWidth;
            return size;
        }

        private void Select(int index)
        {
            SelectCallback?.Invoke(index);
        }

        private void BeginRename(int index)
        {
            if (_root == null || RenameCallback == null || index < 0 || index >= _tabButtons.Count)
            {
                return;
            }

            var tabButton = _tabButtons[index];
            var textField = new TextField
            {
                value = tabButton.Text
            };

            textField.style.width = tabButton.resolvedStyle.width;
            textField.style.height = tabButton.resolvedStyle.height;

            void ApplyRename(bool accept)
            {
                if (accept)
                {
                    RenameCallback(index, textField.value);
                }

                Refresh();
            }

            textField.RegisterCallback<BlurEvent>(_ => ApplyRename(true));
            textField.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Return)
                {
                    ApplyRename(true);
                }
                else if (evt.keyCode == KeyCode.Escape)
                {
                    ApplyRename(false);
                }
            });

            var buttonIndex = _root.IndexOf(tabButton);
            _root.Remove(tabButton);
            _root.Insert(buttonIndex, textField);
            textField.Focus();
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (!Draggable || _dragIndex < 0)
            {
                return;
            }

            var pointerPosition = evt.position;
            for (var index = 0; index < _tabButtons.Count; index++)
            {
                var tab = _tabButtons[index];
                if (!tab.worldBound.Contains(pointerPosition))
                {
                    continue;
                }

                _dragTargetIndex = index;
                _dragAfter = pointerPosition.x > tab.worldBound.center.x;
                UpdateDragIndicator(tab);
                break;
            }
        }

        private void UpdateDragIndicator(VisualElement tab)
        {
            if (_dragIndicator == null)
            {
                return;
            }

            var bounds = tab.worldBound;
            var x = _dragAfter ? bounds.xMax : bounds.xMin;
            _dragIndicator.style.left = x - _root.worldBound.xMin;
            _dragIndicator.style.top = bounds.yMin - _root.worldBound.yMin;
            _dragIndicator.style.height = bounds.height;
            _dragIndicator.style.display = DisplayStyle.Flex;
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if (!Draggable || _dragIndex < 0)
            {
                return;
            }

            Move(_dragIndex, _dragTargetIndex, _dragAfter);
            _dragIndex = -1;
            _dragTargetIndex = -1;
            _dragAfter = false;
            _dragIndicator.style.display = DisplayStyle.None;
        }

        private void Move(int sourceIndex, int targetIndex, bool isAfter)
        {
            if (sourceIndex < 0 || targetIndex < 0 || sourceIndex == targetIndex || sourceIndex >= _elements.Count)
            {
                return;
            }

            var destinationIndex = targetIndex;
            if (sourceIndex > targetIndex)
            {
                if (isAfter)
                {
                    destinationIndex += 1;
                }
            }
            else
            {
                if (!isAfter)
                {
                    destinationIndex -= 1;
                }
            }

            destinationIndex = Mathf.Clamp(destinationIndex, 0, _elements.Count - 1);

            var item = _elements[sourceIndex];
            _elements.RemoveAt(sourceIndex);
            _elements.Insert(destinationIndex, item);

            MoveCallback?.Invoke();
            Refresh();
        }
    }
}
