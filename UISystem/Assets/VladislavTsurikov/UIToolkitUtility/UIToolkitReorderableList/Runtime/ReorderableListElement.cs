#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;

namespace VladislavTsurikov.UIToolkitReorderableList.Runtime
{
    public class ReorderableListElement : VisualElement
    {
        private readonly VisualElement _dragHandle;
        private readonly VisualElement _contentContainer;
        private bool _isDragging;

        public VisualElement ContentContainer => _contentContainer;

        public ReorderableListElement()
        {
            AddToClassList("reorderable-list-element");

            style.flexDirection = FlexDirection.Row;
            style.paddingLeft = 2;
            style.paddingRight = 2;
            style.paddingTop = 1;
            style.paddingBottom = 1;
            style.minHeight = 20;

            // Drag handle
            _dragHandle = new VisualElement();
            _dragHandle.AddToClassList("reorderable-list-element__drag-handle");
            _dragHandle.style.width = 10;
            _dragHandle.style.height = 18;
            _dragHandle.style.marginRight = 4;
            _dragHandle.style.alignSelf = Align.Center;
            _dragHandle.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            _dragHandle.style.borderLeftWidth = 1;
            _dragHandle.style.borderRightWidth = 1;
            _dragHandle.style.borderTopWidth = 1;
            _dragHandle.style.borderBottomWidth = 1;
            _dragHandle.style.borderLeftColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            _dragHandle.style.borderRightColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            _dragHandle.style.borderTopColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            _dragHandle.style.borderBottomColor = new Color(0.3f, 0.3f, 0.3f, 1f);

            // Add grip lines
            for (int i = 0; i < 3; i++)
            {
                var line = new VisualElement();
                line.style.height = 1;
                line.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1f);
                line.style.marginTop = i == 0 ? 4 : 2;
                line.style.marginLeft = 2;
                line.style.marginRight = 2;
                _dragHandle.Add(line);
            }

            Add(_dragHandle);

            // Content container
            _contentContainer = new VisualElement();
            _contentContainer.AddToClassList("reorderable-list-element__content");
            _contentContainer.style.flexGrow = 1;
            Add(_contentContainer);

            RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
        }

        public void SetDragHandleVisible(bool visible)
        {
            _dragHandle.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void OnMouseEnter(MouseEnterEvent evt)
        {
            _dragHandle.style.backgroundColor = new Color(0.6f, 0.6f, 0.6f, 0.7f);
        }

        private void OnMouseLeave(MouseLeaveEvent evt)
        {
            _dragHandle.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
    }
}
#endif
