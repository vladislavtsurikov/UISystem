#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    public sealed class ResourceLoaderTemplateChangeElement : VisualElement
    {
        private ResourceLoaderTemplate _selectedTemplate;
        private ResourceLoaderTemplateElement _templateElement;

        public event Action<ResourceLoaderTemplate> TemplateChanged;

        public ResourceLoaderTemplate SelectedTemplate => _selectedTemplate;

        public ResourceLoaderTemplateChangeElement(ResourceLoaderTemplate initialTemplate)
        {
            _selectedTemplate = initialTemplate != null
                ? initialTemplate
                : ResourceLoaderTemplateTypeRegistry.CreateDefaultInstance();

            Add(CreatePopup());

            _templateElement = new ResourceLoaderTemplateElement();
            _templateElement.SetTemplate(_selectedTemplate);
            Add(_templateElement);

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

            _templateElement.SetTemplate(_selectedTemplate);

            TemplateChanged?.Invoke(_selectedTemplate);
        }
    }
}
#endif
