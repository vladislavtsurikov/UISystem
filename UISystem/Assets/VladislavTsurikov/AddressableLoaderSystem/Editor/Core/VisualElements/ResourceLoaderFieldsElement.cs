using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create
{
    public class ResourceLoaderFieldsElement : VisualElement
    {
        private readonly List<ResourceLoaderFieldData> _fields;
        private readonly VisualElement _listContainer;
        private readonly Button _addButton;

        public ResourceLoaderFieldsElement(List<ResourceLoaderFieldData> fields)
        {
            _fields = fields;
            style.marginTop = 6;
            style.flexDirection = FlexDirection.Column;

            _listContainer = new VisualElement();
            _listContainer.style.flexDirection = FlexDirection.Column;
            Add(_listContainer);

            _addButton = new Button(AddField) { text = "+ Add Field" };
            _addButton.style.marginTop = 4;
            Add(_addButton);

            RefreshList();
        }

        private void RefreshList()
        {
            _listContainer.Clear();

            for (int i = 0; i < _fields.Count; i++)
            {
                int index = i;
                var field = _fields[i];

                var row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                row.style.marginBottom = 4;

                var objectField = new ObjectField
                {
                    value = field.Asset,
                    objectType = typeof(UnityEngine.Object),
                    allowSceneObjects = false
                };
                objectField.style.flexGrow = 1;
                objectField.RegisterValueChangedCallback(evt =>
                {
                    field.Asset = evt.newValue;
                });
                row.Add(objectField);

                var removeButton = new Button(() =>
                {
                    _fields.RemoveAt(index);
                    RefreshList();
                })
                { text = "X" };
                removeButton.style.width = 20;
                removeButton.style.marginLeft = 4;
                row.Add(removeButton);

                _listContainer.Add(row);
            }
        }

        private void AddField()
        {
            _fields.Add(new ResourceLoaderFieldData());
            RefreshList();
        }
    }
}
