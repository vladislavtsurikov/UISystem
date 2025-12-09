#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine.UIElements;
using VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create.ResourceLoaderDescriptorElements;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create
{
    public class ResourceLoaderDescriptorContainerElement : VisualElement
    {
        private readonly ResourceLoaderDescriptorContainer _provider;
        private ResourceLoaderDescriptor _selectedDescriptor;
        private ResourceLoaderDescriptorElement _resourceLoaderDescriptorElement;

        public ResourceLoaderDescriptorElement ResourceLoaderDescriptorElement => _resourceLoaderDescriptorElement;

        public ResourceLoaderDescriptorContainerElement(ResourceLoaderDescriptorContainer provider)
        {
            _provider = provider;

            List<string> baseTypeNames = _provider.GetBaseTypeNames();

            _selectedDescriptor = _provider.ActiveDescriptor;

            _resourceLoaderDescriptorElement = ResourceLoaderDescriptorEditorStack.GetElement(_selectedDescriptor);

            string activeName = _selectedDescriptor != null ? _selectedDescriptor.GetBaseTypeName() : baseTypeNames[0];
            var popup = new PopupField<string>("Base Type", baseTypeNames, activeName);
            popup.value = activeName;
            popup.RegisterValueChangedCallback(evt =>
            {
                _selectedDescriptor = _provider.GetByBaseTypeName(evt.newValue);
                _provider.SetActiveByBaseTypeName(evt.newValue);
                _resourceLoaderDescriptorElement.RefreshForm();
            });
            Add(popup);

            Add(_resourceLoaderDescriptorElement);
        }
    }
}
#endif
