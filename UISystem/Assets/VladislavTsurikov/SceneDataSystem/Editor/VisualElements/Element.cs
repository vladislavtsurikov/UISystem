#if UNITY_EDITOR
#if !DISABLE_VISUAL_ELEMENTS
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using VladislavTsurikov.ColorUtility.Runtime;
using VladislavTsurikov.UIElementsUtility.Editor.Groups.SelectableColors;
using VladislavTsurikov.UIElementsUtility.Runtime;

namespace VladislavTsurikov.SceneDataSystem.Editor.VisualElements
{
    public class Element
    {
        private SelectionState _mSelectionState;
        public UnityAction<EventBase> OnClick;
        public UnityAction<PointerDownEvent> OnPointerDown;
        public UnityAction<PointerEnterEvent> OnPointerEnter;
        public UnityAction<PointerLeaveEvent> OnPointerLeave;
        public UnityAction<PointerUpEvent> OnPointerUp;

        public UnityAction OnStateChanged;

        public Element() => Reset();

        public Element(VisualElement target) : this() => SetTarget(target);

        public bool HasAccentColor => EditorSelectableAccentColor != null;
        public EditorSelectableColorInfo EditorSelectableAccentColor { get; private set; }
        public EditorSelectableColorInfo ButtonContainerEditorSelectableColor { get; private set; }
        public EditorSelectableColorInfo IconSelectableColor { get; private set; }
        public EditorSelectableColorInfo TextEditorSelectableColor { get; private set; }

        public Color ContainerColor { get; set; }
        public Color ContainerBorderColor { get; set; }
        public Color IconColor { get; set; }
        public Color TextColor { get; set; }

        public SelectionState SelectionState
        {
            get => _mSelectionState;
            set
            {
                _mSelectionState = value;
                StateChanged();
            }
        }

        private Clickable Clickable { get; set; }

        public bool HasTarget => Target != null;
        public VisualElement Target { get; private set; }

        public void Reset()
        {
            ButtonContainerEditorSelectableColor = Default.ButtonContainerEditorSelectableColor;
            IconSelectableColor = Default.IconEditorSelectableColor;
            TextEditorSelectableColor = Default.TextEditorSelectableColor;
            EditorSelectableAccentColor = null;

            ClearTarget();

            OnStateChanged = null;
            OnClick = null;
            OnPointerEnter = null;
            OnPointerLeave = null;
            OnPointerDown = null;
            OnPointerUp = null;

            SelectionState = SelectionState.Normal;
        }


        /// <summary> Set an accent selectable color for this element </summary>
        /// <param name="value"> Selectable color info </param>
        public Element SetAccentColor(EditorSelectableColorInfo value)
        {
            EditorSelectableAccentColor = value;
            StateChanged();
            return this;
        }

        public Element ResetAccentColor() => SetAccentColor(null);

        /// <summary> Set target VisualElement and connect to it (null value disconnects) </summary>
        /// <param name="value"> Target VisualElement </param>
        /// <param name="focusable"> True if target can be focused </param>
        public Element SetTarget(VisualElement value, bool focusable = false)
        {
            Clickable = Clickable ?? new Clickable(ExecuteOnClick);

            if (HasTarget)
            {
                //ENTER & EXIT
                Target.UnregisterCallback<PointerEnterEvent>(ExecuteOnPointerEnter);
                Target.UnregisterCallback<PointerLeaveEvent>(ExecuteOnPointerLeave);

                //DOWN & UP
                Target.UnregisterCallback<PointerDownEvent>(ExecuteOnPointerDown);
                Target.UnregisterCallback<PointerUpEvent>(ExecuteOnPointerUp);

                //FOCUS IN & OUT
                Target.UnregisterCallback<FocusInEvent>(ExecuteFocusIn);
                Target.UnregisterCallback<FocusOutEvent>(ExecuteFocusOut);

                //CLICK
                Target.RemoveManipulator(Clickable);
            }

            Target = value;

            if (Target == null)
            {
                SelectionState = SelectionState.Normal;
                return this;
            }

            //ENTER & EXIT
            Target.RegisterCallback<PointerEnterEvent>(ExecuteOnPointerEnter);
            Target.RegisterCallback<PointerLeaveEvent>(ExecuteOnPointerLeave);

            //DOWN & UP
            Target.RegisterCallback<PointerDownEvent>(ExecuteOnPointerDown);
            Target.RegisterCallback<PointerUpEvent>(ExecuteOnPointerUp);

            //FOCUS IN & OUT
            Target.RegisterCallback<FocusInEvent>(ExecuteFocusIn);
            Target.RegisterCallback<FocusOutEvent>(ExecuteFocusOut);
            Target.focusable = focusable;

            //CLICK
            Target.AddManipulator(Clickable);

            SelectionState = SelectionState.Normal;

            // target.schedule.Execute(() => selectionState = SelectionState.Normal);
            return this;
        }

        /// <summary> Remove target VisualElement and disconnect from it </summary>
        public Element ClearTarget() => SetTarget(null);

        /// <summary> Trigger a state change </summary>
        public void StateChanged()
        {
            ContainerColor = ButtonContainerEditorSelectableColor.GetColor(SelectionState);
            ContainerBorderColor = ContainerColor.WithRGBShade(0.2f);
            IconColor = HasAccentColor
                ? EditorSelectableAccentColor.NormalColor
                : IconSelectableColor.GetColor(SelectionState);
            TextColor = HasAccentColor
                ? EditorSelectableAccentColor.NormalColor
                : TextEditorSelectableColor.GetColor(SelectionState);

            switch (SelectionState)
            {
                case SelectionState.Normal:
                    break;
                case SelectionState.Highlighted:
                    break;
                case SelectionState.Pressed:
                    break;
                case SelectionState.Selected:
                    IconColor = HasAccentColor ? EditorSelectableAccentColor.NormalColor.gamma : IconColor;
                    TextColor = HasAccentColor ? EditorSelectableAccentColor.SelectedColor.gamma : TextColor;
                    break;
                case SelectionState.Disabled:
                    const float alpha = 0.6f;
                    IconColor = HasAccentColor ? EditorSelectableAccentColor.NormalColor.WithAlpha(alpha) : IconColor;
                    ContainerBorderColor = ButtonContainerEditorSelectableColor.PressedColor.WithAlpha(alpha);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            OnStateChanged?.Invoke();
        }

        public void ExecuteFocusIn(FocusInEvent evt = null)
        {
            if (SelectionState == SelectionState.Disabled)
            {
                return;
            }

            SelectionState = SelectionState.Selected;
        }

        public void ExecuteFocusOut(FocusOutEvent evt = null)
        {
            if (SelectionState == SelectionState.Disabled)
            {
                return;
            }

            SelectionState = SelectionState.Normal;
        }

        public void ExecuteOnClick(EventBase clickEvent = null)
        {
            if (SelectionState == SelectionState.Disabled)
            {
                return;
            }

            OnClick?.Invoke(clickEvent);
        }

        public void ExecuteOnPointerEnter(PointerEnterEvent enterEvent = null)
        {
            if (SelectionState == SelectionState.Disabled)
            {
                return;
            }

            SelectionState = SelectionState.Highlighted;
            OnPointerEnter?.Invoke(enterEvent);
        }

        public void ExecuteOnPointerLeave(PointerLeaveEvent leaveEvent = null)
        {
            if (SelectionState == SelectionState.Disabled)
            {
                return;
            }

            SelectionState =
                HasTarget && Target.focusController.focusedElement == Target
                    ? SelectionState.Selected
                    : SelectionState.Normal;
            OnPointerLeave?.Invoke(leaveEvent);
        }

        public void ExecuteOnPointerDown(PointerDownEvent downEvent = null)
        {
            if (SelectionState == SelectionState.Disabled)
            {
                return;
            }

            SelectionState = SelectionState.Pressed;
            OnPointerDown?.Invoke(downEvent);
        }

        public void ExecuteOnPointerUp(PointerUpEvent upEvent = null)
        {
            if (SelectionState == SelectionState.Disabled)
            {
                return;
            }

            if (upEvent != null && HasTarget && Target.ContainsPoint(upEvent.localPosition))
            {
                SelectionState = SelectionState.Highlighted;
            }

            if (upEvent != null && HasTarget && !Target.ContainsPoint(upEvent.localPosition) &&
                SelectionState == SelectionState.Pressed)
            {
                SelectionState = SelectionState.Normal;
            }

            OnPointerUp?.Invoke(upEvent);
        }

        public Element Enable()
        {
            Target?.SetEnabled(true);
            SelectionState = SelectionState.Normal;
            return this;
        }

        public Element Disable()
        {
            Target?.SetEnabled(false);
            SelectionState = SelectionState.Disabled;
            return this;
        }

        public static class Default
        {
            public static EditorSelectableColorInfo ButtonContainerEditorSelectableColor =>
                GetSelectableColor.EditorUI.ButtonContainer;

            public static EditorSelectableColorInfo IconEditorSelectableColor => GetSelectableColor.EditorUI.ButtonIcon;
            public static EditorSelectableColorInfo TextEditorSelectableColor => GetSelectableColor.EditorUI.ButtonText;
        }
    }
}
#endif
#endif
