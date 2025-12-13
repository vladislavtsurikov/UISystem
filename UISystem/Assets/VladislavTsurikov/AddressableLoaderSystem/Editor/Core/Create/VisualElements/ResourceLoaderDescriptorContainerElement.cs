#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine.UIElements;
using VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create.ResourceLoaderDescriptorElements;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create
{
    public class ResourceLoaderDescriptorContainerElement : VisualElement
    {
        private readonly ResourceLoaderDescriptorContainer _provider;
        private ResourceLoaderTemplate _selectedTemplate;
        private ResourceLoaderDescriptorElement _resourceLoaderDescriptorElement;

        public ResourceLoaderDescriptorElement ResourceLoaderDescriptorElement => _resourceLoaderDescriptorElement;

        public ResourceLoaderDescriptorContainerElement(ResourceLoaderDescriptorContainer provider)
        {
            _provider = provider;

            List<string> baseTypeNames = _provider.GetBaseTypeNames();

            _selectedTemplate = _provider.ActiveTemplate;

            _resourceLoaderDescriptorElement = ResourceLoaderDescriptorEditorStack.GetElement(_selectedTemplate);

            string activeName = _selectedTemplate != null ? _selectedTemplate.GetBaseTypeName() : baseTypeNames[0];
            var popup = new PopupField<string>("Base Type", baseTypeNames, activeName);
            popup.value = activeName;
            popup.RegisterValueChangedCallback(evt =>
            {
                _selectedTemplate = _provider.GetByBaseTypeName(evt.newValue);
                _provider.SetActiveByBaseTypeName(evt.newValue);
                _resourceLoaderDescriptorElement.RefreshForm();
            });
            Add(popup);

            Add(_resourceLoaderDescriptorElement);
        }
    }
}
#endif
