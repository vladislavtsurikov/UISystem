#if UNITY_EDITOR
using System.Linq;
using UnityEngine;
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

            var baseTypeNames = _provider.GetBaseTypeNames();
            if (baseTypeNames.Count == 0)
            {
                var info = new Label("No ResourceLoaderGenerator found");
                Debug.LogWarning("No ResourceLoaderGenerator found");
                info.style.color = new Color(1f, 0.5f, 0.5f);
                Add(info);
                return;
            }

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
