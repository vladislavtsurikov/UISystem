#if UNITY_EDITOR
#if !DISABLE_VISUAL_ELEMENTS
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.Events;

namespace VladislavTsurikov.SceneDataSystem.Editor.VisualElements
{
    public class PropertyListViewItem : ListViewItem
    {
        public UnityAction<SerializedProperty> OnRemoveButtonClick;

        public PropertyListViewItem(ListView listView) : base(listView)
        {
            ItemContentContainer.Clear();
            ItemContentContainer.Add(PropertyField = new PropertyField());
        }

        public PropertyField PropertyField { get; }

        public void Update(int index, SerializedProperty property)
        {
            //UPDATE INDEX
            ShowItemIndex = ListView.ShowItemIndex;
            UpdateItemIndex(index);

            //UPDATE PROPERTY
            PropertyField.BindProperty(property);

            //UPDATE REMOVE BUTTON
            ItemRemoveButton.OnClick = () => OnRemoveButtonClick?.Invoke(property);
        }
    }
}
#endif
#endif
