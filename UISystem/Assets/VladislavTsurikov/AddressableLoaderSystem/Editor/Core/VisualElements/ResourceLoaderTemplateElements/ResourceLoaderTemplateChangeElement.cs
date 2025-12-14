#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    public sealed class ResourceLoaderTemplateChangeElement : VisualElement
    {
        private ResourceLoaderTemplate _selectedTemplate;
        private ResourceLoaderTemplateEditorStackElement _templateEditorStackElement;

        public event Action<ResourceLoaderTemplate> TemplateChanged;

        public ResourceLoaderTemplate SelectedTemplate => _selectedTemplate;

        public ResourceLoaderTemplateChangeElement(ResourceLoaderTemplate initialTemplate)
        {
            _selectedTemplate = initialTemplate != null
                ? initialTemplate
                : ResourceLoaderTemplateTypeRegistry.CreateDefaultInstance();

            Add(CreatePopup());

            _templateEditorStackElement = new ResourceLoaderTemplateEditorStackElement();
            _templateEditorStackElement.SetTemplate(_selectedTemplate);
            Add(_templateEditorStackElement);

            TemplateChanged?.Invoke(_selectedTemplate);
        }

        private PopupField<string> CreatePopup()
        {
            List<string> baseTypeNames = ResourceLoaderBaseType.GetBaseTypeNames();

            string initialValue = _selectedTemplate != null ? _selectedTemplate.GetBaseTypeName() : baseTypeNames[0];

            PopupField<string> popup = new PopupField<string>("Base Type", baseTypeNames, initialValue);
            popup.value = initialValue;
            popup.RegisterValueChangedCallback(OnPopupChanged);
            return popup;
        }

        private void OnPopupChanged(ChangeEvent<string> evt)
        {
            ResourceLoaderTemplate newTemplate = ResourceLoaderTemplateTypeRegistry.CreateByBaseTypeName(evt.newValue);
            if (newTemplate == null)
            {
                return;
            }

            _selectedTemplate = newTemplate;

            _templateEditorStackElement.SetTemplate(_selectedTemplate);

            TemplateChanged?.Invoke(_selectedTemplate);
        }
    }
}
#endif
