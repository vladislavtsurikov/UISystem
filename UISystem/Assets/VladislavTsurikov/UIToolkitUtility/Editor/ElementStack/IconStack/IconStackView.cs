#if UNITY_EDITOR
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.UIToolkitUtility.Runtime.ElementStack.IconStack;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.UIToolkitUtility.Editor.ElementStack.IconStack
{
    public class IconStackView : VisualElement
    {
        public delegate IShowIcon AddIconCallbackDelegate(Object obj);
        public delegate void DrawIconDelegate(IconElement iconElement, IShowIcon icon);
        public delegate void DrawWindowMenuDelegate();
        public delegate void IconMenuCallbackDelegate(IShowIcon icon, GenericMenu menu);
        public delegate void IconSelectedDelegate(IShowIcon icon);
        public delegate void SetIconColorDelegate(IShowIcon icon, out Color textColor, out Color backgroundColor);

        private readonly ScrollView _scrollView;
        private readonly VisualElement _iconContainer;
        private readonly Label _emptyLabel;
        private readonly bool _draggable;
        private Type _iconType;

        public AddIconCallbackDelegate AddIconCallback;
        public bool DraggedIconType = false;
        public DrawIconDelegate DrawIcon;
        public DrawWindowMenuDelegate DrawWindowMenu;
        public SetIconColorDelegate IconColor;
        public int IconHeight = 95;
        public IconMenuCallbackDelegate IconMenuCallback;
        public IconSelectedDelegate IconSelected;
        public int IconWidth = 80;
        public string ZeroIconsWarning = "Has no icons";

        public IconStackView(bool draggable = false)
        {
            _draggable = draggable;

            style.minHeight = 100;
            style.flexGrow = 1;

            _scrollView = new ScrollView(ScrollViewMode.Vertical);
            _scrollView.style.flexGrow = 1;
            Add(_scrollView);

            _iconContainer = new VisualElement();
            _iconContainer.style.flexDirection = FlexDirection.Row;
            _iconContainer.style.flexWrap = Wrap.Wrap;
            _iconContainer.style.paddingTop = 4;
            _iconContainer.style.paddingLeft = 4;
            _iconContainer.style.paddingRight = 4;
            _iconContainer.style.paddingBottom = 4;
            _scrollView.Add(_iconContainer);

            _emptyLabel = new Label(ZeroIconsWarning);
            _emptyLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            _emptyLabel.style.flexGrow = 1;
            _emptyLabel.style.display = DisplayStyle.None;
            Add(_emptyLabel);

            RegisterCallback<ContextClickEvent>(OnContextClick);
        }

        public void SetItems(IList elements, Type iconType)
        {
            _iconType = iconType;

            MissingIconsWarningAttribute missingIconsWarningAttribute =
                iconType.GetAttribute<MissingIconsWarningAttribute>();

            if (missingIconsWarningAttribute != null)
            {
                ZeroIconsWarning = missingIconsWarningAttribute.Text;
                _emptyLabel.text = ZeroIconsWarning;
            }

            Refresh(elements);
        }

        public void Refresh(IList elements)
        {
            _iconContainer.Clear();

            if (elements == null || elements.Count == 0)
            {
                _scrollView.style.display = DisplayStyle.None;
                _emptyLabel.style.display = DisplayStyle.Flex;
                return;
            }

            if (elements[^1] is not IShowIcon)
            {
                _scrollView.style.display = DisplayStyle.None;
                _emptyLabel.text = "This List does not have a base type \"IShowIcon\"";
                _emptyLabel.style.display = DisplayStyle.Flex;
                return;
            }

            _scrollView.style.display = DisplayStyle.Flex;
            _emptyLabel.style.display = DisplayStyle.None;

            for (var index = 0; index < elements.Count; index++)
            {
                var icon = (IShowIcon)elements[index];
                var iconElement = CreateIconElement(icon, index, elements);
                _iconContainer.Add(iconElement);
            }

            SetupDragAndDrop(elements);
        }

        private IconElement CreateIconElement(IShowIcon icon, int index, IList elements)
        {
            var iconElement = new IconElement(icon, IconWidth, IconHeight);

            Color textColor;
            Color backgroundColor;

            if (IconColor != null)
            {
                IconColor(icon, out textColor, out backgroundColor);
            }
            else
            {
                SetDefaultIconColor(icon, out textColor, out backgroundColor);
            }

            iconElement.SetColors(textColor, backgroundColor);

            if (DrawIcon != null)
            {
                DrawIcon(iconElement, icon);
            }
            else
            {
                iconElement.SetDefaultContent(icon.PreviewTexture, icon.Name);
            }

            iconElement.RegisterCallback<MouseDownEvent>(evt => HandleSelect(elements, index, evt));

            if (IconMenuCallback != null)
            {
                iconElement.RegisterCallback<ContextClickEvent>(evt =>
                {
                    var menu = new GenericMenu();
                    IconMenuCallback(icon, menu);
                    menu.ShowAsContext();
                    evt.StopPropagation();
                });
            }

            if (_draggable)
            {
                iconElement.SetDraggable(true, () => icon);
                iconElement.RegisterCallback<DragUpdatedEvent>(evt => OnDragUpdate(evt, elements, index));
                iconElement.RegisterCallback<DragPerformEvent>(evt => OnDragPerform(evt, elements, index));
            }

            return iconElement;
        }

        private void SetDefaultIconColor(IShowIcon icon, out Color textColor, out Color backgroundColor)
        {
            textColor = Color.white;

            if (icon.Selected)
            {
                backgroundColor = icon.IsRedIcon
                    ? new Color(0.8f, 0.2f, 0.2f, 1f)
                    : new Color(0.24f, 0.48f, 0.71f, 1f);
            }
            else
            {
                backgroundColor = icon.IsRedIcon
                    ? new Color(0.4f, 0.1f, 0.1f, 1f)
                    : new Color(0.22f, 0.22f, 0.22f, 1f);
            }
        }

        private void HandleSelect(IList elements, int index, MouseDownEvent evt)
        {
            if (evt.button != 0) return;

            if (evt.ctrlKey || evt.commandKey)
            {
                SelectAdditive(elements, index);
            }
            else if (evt.shiftKey)
            {
                SelectRange(elements, index);
            }
            else
            {
                Select(elements, index);
            }

            evt.StopPropagation();
        }

        private void Select(IList elements, int index)
        {
            if (index < 0 || index >= elements.Count) return;

            foreach (IShowIcon icon in elements)
            {
                icon.Selected = false;
                IconSelected?.Invoke(icon);
            }

            var selectedIcon = (IShowIcon)elements[index];
            selectedIcon.Selected = true;
            IconSelected?.Invoke(selectedIcon);

            Refresh(elements);
        }

        private void SelectAdditive(IList elements, int index)
        {
            if (index < 0 || index >= elements.Count) return;

            var icon = (IShowIcon)elements[index];
            icon.Selected = !icon.Selected;
            IconSelected?.Invoke(icon);

            Refresh(elements);
        }

        private void SelectRange(IList elements, int index)
        {
            if (index < 0 || index >= elements.Count) return;

            var rangeMin = index;
            var rangeMax = index;

            for (var i = 0; i < elements.Count; i++)
            {
                var icon = (IShowIcon)elements[i];
                if (icon.Selected)
                {
                    rangeMin = Mathf.Min(rangeMin, i);
                    rangeMax = Mathf.Max(rangeMax, i);
                }
            }

            for (var i = rangeMin; i <= rangeMax; i++)
            {
                var icon = (IShowIcon)elements[i];
                icon.Selected = true;
                IconSelected?.Invoke(icon);
            }

            Refresh(elements);
        }

        private void SetupDragAndDrop(IList elements)
        {
            if (AddIconCallback == null) return;

            _scrollView.RegisterCallback<DragUpdatedEvent>(evt =>
            {
                if (DragAndDrop.objectReferences.Length > 0)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                }
            });

            _scrollView.RegisterCallback<DragPerformEvent>(evt =>
            {
                DragAndDrop.AcceptDrag();

                DropObjectsAttribute dropObjectsAttribute = _iconType?.GetAttribute<DropObjectsAttribute>();

                foreach (Object draggedObject in DragAndDrop.objectReferences)
                {
                    if (DraggedIconType)
                    {
                        if (_iconType != null && _iconType.IsInstanceOfType(draggedObject))
                        {
                            AddIconCallback(draggedObject);
                        }
                    }
                    else if (dropObjectsAttribute != null)
                    {
                        foreach (Type type in dropObjectsAttribute.ObjectsTypes)
                        {
                            if (type == typeof(GameObject))
                            {
                                if (draggedObject is GameObject &&
                                    PrefabUtility.GetPrefabAssetType(draggedObject as GameObject) !=
                                    PrefabAssetType.NotAPrefab &&
                                    AssetDatabase.Contains(draggedObject))
                                {
                                    AddIconCallback(draggedObject);
                                }
                            }
                            else
                            {
                                if (type == draggedObject.GetType())
                                {
                                    AddIconCallback(draggedObject);
                                }
                            }
                        }
                    }
                }

                Refresh(elements);
            });
        }

        private void OnDragUpdate(DragUpdatedEvent evt, IList elements, int targetIndex)
        {
            if (!_draggable) return;

            var draggedIcon = DragAndDrop.GetGenericData("icon") as IShowIcon;
            if (draggedIcon != null && _iconType != null && draggedIcon.GetType() == _iconType)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            }
        }

        private void OnDragPerform(DragPerformEvent evt, IList elements, int targetIndex)
        {
            if (!_draggable) return;

            var draggedIcon = DragAndDrop.GetGenericData("icon") as IShowIcon;
            if (draggedIcon != null)
            {
                var sourceIndex = GetIconIndex(elements, draggedIcon);
                if (sourceIndex >= 0 && sourceIndex != targetIndex)
                {
                    MoveIcon(elements, sourceIndex, targetIndex);
                    Refresh(elements);
                }
            }
        }

        private int GetIconIndex(IList elements, IShowIcon icon)
        {
            for (var i = 0; i < elements.Count; i++)
            {
                if (elements[i] == icon) return i;
            }
            return -1;
        }

        private void MoveIcon(IList elements, int sourceIndex, int targetIndex)
        {
            if (targetIndex >= elements.Count || sourceIndex == targetIndex) return;

            var item = elements[sourceIndex];
            elements.RemoveAt(sourceIndex);

            if (sourceIndex < targetIndex)
            {
                targetIndex--;
            }

            elements.Insert(targetIndex, item);
        }

        private void OnContextClick(ContextClickEvent evt)
        {
            if (DrawWindowMenu != null)
            {
                DrawWindowMenu();
                evt.StopPropagation();
            }
        }
    }

    public class IconElement : VisualElement
    {
        private readonly VisualElement _background;
        private readonly VisualElement _previewContainer;
        private readonly Label _nameLabel;
        private readonly IShowIcon _icon;

        public IconElement(IShowIcon icon, int width, int height)
        {
            _icon = icon;

            style.width = width;
            style.height = height;
            style.marginRight = 4;
            style.marginBottom = 4;

            _background = new VisualElement();
            _background.style.flexGrow = 1;
            _background.style.borderTopLeftRadius = 2;
            _background.style.borderTopRightRadius = 2;
            _background.style.borderBottomLeftRadius = 2;
            _background.style.borderBottomRightRadius = 2;
            Add(_background);

            _previewContainer = new VisualElement();
            _previewContainer.style.position = Position.Absolute;
            _previewContainer.style.left = 2;
            _previewContainer.style.top = 2;
            _previewContainer.style.right = 2;
            _previewContainer.style.bottom = 20;
            _background.Add(_previewContainer);

            _nameLabel = new Label();
            _nameLabel.style.position = Position.Absolute;
            _nameLabel.style.bottom = 2;
            _nameLabel.style.left = 2;
            _nameLabel.style.right = 2;
            _nameLabel.style.height = 16;
            _nameLabel.style.unityTextAlign = TextAnchor.LowerCenter;
            _nameLabel.style.fontSize = 10;
            _nameLabel.style.overflow = Overflow.Hidden;
            _nameLabel.style.textOverflow = TextOverflow.Ellipsis;
            _background.Add(_nameLabel);
        }

        public void SetColors(Color textColor, Color backgroundColor)
        {
            _background.style.backgroundColor = backgroundColor;
            _nameLabel.style.color = textColor;
        }

        public void SetDefaultContent(Texture2D preview, string name)
        {
            _previewContainer.Clear();

            if (preview != null)
            {
                var previewImage = new Image { image = preview };
                previewImage.style.flexGrow = 1;
                previewImage.scaleMode = ScaleMode.ScaleToFit;
                _previewContainer.Add(previewImage);
            }
            else
            {
                var emptyBox = new VisualElement();
                emptyBox.style.flexGrow = 1;
                emptyBox.style.backgroundColor = new Color(0.4f, 0.4f, 0.4f, 1f);
                _previewContainer.Add(emptyBox);
            }

            _nameLabel.text = GetShortName(name);
            _nameLabel.tooltip = name;
        }

        public void SetDraggable(bool draggable, Func<IShowIcon> getIcon)
        {
            if (!draggable) return;

            RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.button == 0)
                {
                    DragAndDrop.PrepareStartDrag();
                    DragAndDrop.SetGenericData("icon", getIcon());
                    DragAndDrop.StartDrag("Dragging Icon");
                }
            });
        }

        private string GetShortName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName)) return "";

            const int maxLength = 12;
            if (fullName.Length <= maxLength) return fullName;

            return fullName.Substring(0, maxLength - 2) + "..";
        }
    }
}
#endif
