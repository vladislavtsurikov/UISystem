#if UNITY_EDITOR
#if !DISABLE_VISUAL_ELEMENTS
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.UIElementsUtility.Runtime;
using VladislavTsurikov.UIElementsUtility.Runtime.Utility;
using VisualElementExtensions = VladislavTsurikov.UIElementsUtility.Runtime.Utility.VisualElementExtensions;

namespace VladislavTsurikov.SceneDataSystem.Editor.VisualElements
{
    public abstract class ListViewItem : VisualElement
    {
        protected ListViewItem()
        {
            this.SetStyleJustifyContent(Justify.Center);

            Add(TemplateContainer = GetLayout.VisualElements.ListViewItem.CloneTree());
            TemplateContainer
                .AddStyle(GetStyle.VisualElements.ListViewItem);

            //REFERENCES
            LayoutContainer = TemplateContainer.Q<VisualElement>("LayoutContainer");
            ItemIndexLabel = LayoutContainer.Q<Label>("ItemIndex");
            ItemContentContainer = LayoutContainer.Q<VisualElement>("ItemContentContainer");
            ItemRemoveButtonContainer = LayoutContainer.Q<VisualElement>("ItemRemoveButtonContainer");

            //TEXT COLORS
            ItemIndexLabel.SetStyleColor(ItemIndexTextColor);

            //TEXT FONTS
            ItemIndexLabel.SetStyleUnityFont(IndexLabelFont);
        }

        protected ListViewItem(ListView listView) : this()
        {
            this.SetListView(listView);
            ItemRemoveButtonContainer.Clear();
            ItemRemoveButton = ListView.Buttons.RemoveButton;
            ItemRemoveButtonContainer.Add(ItemRemoveButton);
        }

        //REFERENCES
        public TemplateContainer TemplateContainer { get; }
        public VisualElement LayoutContainer { get; }
        public Label ItemIndexLabel { get; }
        public VisualElement ItemContentContainer { get; }
        public VisualElement ItemRemoveButtonContainer { get; }

        //SETTINGS
        public ListView ListView { get; internal set; }
        public int ItemIndex { get; protected set; }
        public Button ItemRemoveButton { get; }

        public bool ShowItemIndex { get; protected set; }
        public bool ShowRemoveButton { get; protected set; }

        private static Color ItemIndexTextColor => GetEditorColor.EditorUI.TextDescription;

        private static Font IndexLabelFont => GetFont.Ubuntu.Light;

        internal virtual void UpdateItemIndex(int value)
        {
            ItemIndexLabel.SetStyleDisplay(ShowItemIndex ? DisplayStyle.Flex : DisplayStyle.None);
            value = Mathf.Max(0, value);
            ItemIndex = value;
            ItemIndexLabel.text = value.ToString();
        }
    }

    public static class FluidListViewItemExtensions
    {
        internal static T SetListView<T>(this T target, ListView value) where T : ListViewItem
        {
            target.ListView = value;
            return target;
        }

        public static T SetItemIndex<T>(this T target, int value) where T : ListViewItem
        {
            target.UpdateItemIndex(value);
            return target;
        }

        public static T EnableItemRemoveButton<T>(this T target) where T : ListViewItem
        {
            VisualElementExtensions.EnableElement(target.ItemRemoveButton);
            return target;
        }

        public static T DisableItemRemoveButton<T>(this T target) where T : ListViewItem
        {
            VisualElementExtensions.DisableElement(target.ItemRemoveButton);
            return target;
        }

        public static T ToggleItemRemoveButton<T>(this T target, bool enabled) where T : ListViewItem =>
            enabled
                ? target.EnableItemRemoveButton()
                : target.DisableItemRemoveButton();
    }
}
#endif
#endif
