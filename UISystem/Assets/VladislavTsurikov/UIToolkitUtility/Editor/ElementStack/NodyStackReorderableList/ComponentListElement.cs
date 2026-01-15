#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack.UIToolkitReorderableList
{
    public class ComponentListElement : VisualElement
    {
        private VisualElement _headerContainer;
        private VisualElement _dragHandle;
        private Foldout _foldout;
        private Toggle _activeToggle;
        private Label _headerLabel;
        private Button _menuButton;
        private VisualElement _contentContainer;
        private VisualElement _renameContainer;
        private TextField _renameField;
        private Button _renameOkButton;
        private Button _renameCancelButton;

        private Node _component;
        private bool _isSelected;
        private bool _isRenaming;

        public Action<ComponentListElement> OnMenuClicked;
        public Action<ComponentListElement> OnSelectionChanged;
        public Action<ComponentListElement, bool> OnActiveChanged;
        public Action<ComponentListElement, string> OnRenameComplete;

        public Node Component => _component;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    UpdateSelectedState();
                }
            }
        }

        public bool ShowActiveToggle { get; set; } = true;

        public ComponentListElement()
        {
            AddToClassList("component-list-element");

            style.marginBottom = 1;

            // Main container with background
            var mainContainer = new VisualElement();
            mainContainer.AddToClassList("component-list-element__main");
            mainContainer.style.backgroundColor = new Color(0.22f, 0.22f, 0.22f, 1f);
            mainContainer.style.borderLeftWidth = 1;
            mainContainer.style.borderRightWidth = 1;
            mainContainer.style.borderTopWidth = 1;
            mainContainer.style.borderBottomWidth = 1;
            mainContainer.style.borderLeftColor = new Color(0.12f, 0.12f, 0.12f, 1f);
            mainContainer.style.borderRightColor = new Color(0.12f, 0.12f, 0.12f, 1f);
            mainContainer.style.borderTopColor = new Color(0.12f, 0.12f, 0.12f, 1f);
            mainContainer.style.borderBottomColor = new Color(0.12f, 0.12f, 0.12f, 1f);

            Add(mainContainer);

            // Header container
            _headerContainer = new VisualElement();
            _headerContainer.AddToClassList("component-list-element__header");
            _headerContainer.style.flexDirection = FlexDirection.Row;
            _headerContainer.style.alignItems = Align.Center;
            _headerContainer.style.minHeight = 20;
            _headerContainer.style.paddingLeft = 4;
            _headerContainer.style.paddingRight = 4;
            _headerContainer.style.paddingTop = 2;
            _headerContainer.style.paddingBottom = 2;

            mainContainer.Add(_headerContainer);

            // Drag handle
            _dragHandle = CreateDragHandle();
            _headerContainer.Add(_dragHandle);

            // Foldout
            _foldout = new Foldout();
            _foldout.text = "";
            _foldout.value = false;
            _foldout.style.flexGrow = 1;
            _foldout.style.marginLeft = 0;
            _foldout.style.marginRight = 0;

            // Custom header for foldout
            var foldoutToggle = _foldout.Q<Toggle>();
            if (foldoutToggle != null)
            {
                foldoutToggle.style.flexDirection = FlexDirection.Row;
                foldoutToggle.style.flexGrow = 1;
            }

            _headerContainer.Add(_foldout);

            // Header label
            _headerLabel = new Label("Component");
            _headerLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            _headerLabel.style.flexGrow = 1;
            _headerLabel.style.marginLeft = 4;
            _foldout.Q<Toggle>()?.Add(_headerLabel);

            // Active toggle
            _activeToggle = new Toggle();
            _activeToggle.style.marginLeft = 4;
            _activeToggle.style.marginRight = 4;
            _activeToggle.RegisterValueChangedCallback(evt =>
            {
                OnActiveChanged?.Invoke(this, evt.newValue);
            });
            _foldout.Q<Toggle>()?.Add(_activeToggle);

            // Menu button
            _menuButton = new Button(() => OnMenuClicked?.Invoke(this));
            _menuButton.text = "⋮";
            _menuButton.style.width = 20;
            _menuButton.style.height = 18;
            _menuButton.style.fontSize = 14;
            _menuButton.style.unityTextAlign = TextAnchor.MiddleCenter;
            _menuButton.style.marginLeft = 2;
            _foldout.Q<Toggle>()?.Add(_menuButton);

            // Content container
            _contentContainer = new VisualElement();
            _contentContainer.AddToClassList("component-list-element__content");
            _contentContainer.style.paddingLeft = 20;
            _contentContainer.style.paddingRight = 8;
            _contentContainer.style.paddingTop = 4;
            _contentContainer.style.paddingBottom = 4;
            _foldout.Add(_contentContainer);

            // Rename container
            _renameContainer = CreateRenameContainer();
            mainContainer.Add(_renameContainer);

            // Click handler for selection
            RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.button == 0 && !_isRenaming)
                {
                    IsSelected = true;
                    OnSelectionChanged?.Invoke(this);
                    evt.StopPropagation();
                }
            });

            UpdateSelectedState();
        }

        private VisualElement CreateDragHandle()
        {
            var handle = new VisualElement();
            handle.AddToClassList("component-list-element__drag-handle");
            handle.style.width = 10;
            handle.style.height = 14;
            handle.style.marginRight = 4;
            handle.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            handle.style.borderLeftWidth = 1;
            handle.style.borderRightWidth = 1;
            handle.style.borderTopWidth = 1;
            handle.style.borderBottomWidth = 1;
            handle.style.borderLeftColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            handle.style.borderRightColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            handle.style.borderTopColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            handle.style.borderBottomColor = new Color(0.3f, 0.3f, 0.3f, 1f);

            // Add grip lines
            for (int i = 0; i < 2; i++)
            {
                var line = new VisualElement();
                line.style.height = 1;
                line.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1f);
                line.style.marginTop = i == 0 ? 3 : 2;
                line.style.marginLeft = 2;
                line.style.marginRight = 2;
                handle.Add(line);
            }

            return handle;
        }

        private VisualElement CreateRenameContainer()
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;
            container.style.paddingLeft = 8;
            container.style.paddingRight = 8;
            container.style.paddingTop = 4;
            container.style.paddingBottom = 4;
            container.style.display = DisplayStyle.None;

            var label = new Label("Rename to:");
            label.style.marginRight = 4;
            container.Add(label);

            _renameField = new TextField();
            _renameField.style.flexGrow = 1;
            container.Add(_renameField);

            _renameOkButton = new Button(() => CompleteRename(true));
            _renameOkButton.text = "✓";
            _renameOkButton.style.width = 20;
            _renameOkButton.style.backgroundColor = new Color(0.2f, 0.6f, 0.2f, 1f);
            _renameOkButton.style.marginLeft = 4;
            container.Add(_renameOkButton);

            _renameCancelButton = new Button(() => CompleteRename(false));
            _renameCancelButton.text = "✗";
            _renameCancelButton.style.width = 20;
            _renameCancelButton.style.backgroundColor = new Color(0.6f, 0.2f, 0.2f, 1f);
            _renameCancelButton.style.marginLeft = 2;
            container.Add(_renameCancelButton);

            _renameField.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                {
                    CompleteRename(true);
                    evt.StopPropagation();
                }
                else if (evt.keyCode == KeyCode.Escape)
                {
                    CompleteRename(false);
                    evt.StopPropagation();
                }
            });

            return container;
        }

        public void SetComponent(Node component, bool showActiveToggle)
        {
            _component = component;
            ShowActiveToggle = showActiveToggle;

            if (component != null)
            {
                _headerLabel.text = component.Name;
                _activeToggle.value = component.Active;
                _foldout.value = component.SelectSettingsFoldout;
                _activeToggle.style.display = showActiveToggle && component.ShowActiveToggle()
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;

                // Register foldout change
                _foldout.RegisterValueChangedCallback(evt =>
                {
                    component.SelectSettingsFoldout = evt.newValue;
                });
            }
        }

        public void SetContent(VisualElement content)
        {
            _contentContainer.Clear();
            if (content != null)
            {
                _contentContainer.Add(content);
            }
        }

        public void SetDragHandleVisible(bool visible)
        {
            _dragHandle.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void StartRename()
        {
            if (_component == null)
                return;

            _isRenaming = true;
            _renameField.value = _component.Name;
            _headerContainer.style.display = DisplayStyle.None;
            _foldout.style.display = DisplayStyle.None;
            _renameContainer.style.display = DisplayStyle.Flex;
            _renameField.Focus();
        }

        private void CompleteRename(bool accept)
        {
            _isRenaming = false;
            _headerContainer.style.display = DisplayStyle.Flex;
            _foldout.style.display = DisplayStyle.Flex;
            _renameContainer.style.display = DisplayStyle.None;

            if (accept && !string.IsNullOrWhiteSpace(_renameField.value))
            {
                OnRenameComplete?.Invoke(this, _renameField.value);
            }
        }

        private void UpdateSelectedState()
        {
            if (_isSelected)
            {
                style.backgroundColor = new Color(0.26f, 0.26f, 0.26f, 1f);
                style.borderLeftColor = new Color(0.27f, 0.57f, 0.83f, 1f);
                style.borderLeftWidth = 2;
            }
            else
            {
                style.backgroundColor = new Color(0.22f, 0.22f, 0.22f, 1f);
                style.borderLeftColor = new Color(0.12f, 0.12f, 0.12f, 1f);
                style.borderLeftWidth = 1;
            }
        }
    }
}
#endif
