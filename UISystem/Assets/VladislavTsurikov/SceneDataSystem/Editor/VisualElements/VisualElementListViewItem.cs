#if UNITY_EDITOR
#if !DISABLE_VISUAL_ELEMENTS
using UnityEngine.UIElements;

namespace VladislavTsurikov.SceneDataSystem.Editor.VisualElements
{
    public sealed class VisualElementListViewItem : ListViewItem
    {
        public VisualElementListViewItem(VisualElement visualElement, int index, ListView listView) : base(listView)
        {
            ShowItemIndex = true;
            ItemContentContainer.Clear();
            ItemContentContainer.Add(visualElement);
            UpdateItemIndex(index);
        }
    }
}
#endif
#endif
