#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create
{
    public class CreateSectionElement : VisualElement
    {
        private readonly ResourceLoaderDescriptorContainer _provider;
        private ResourceLoaderDescriptor _selectedDescriptor;
        private Button _createButton;

        public CreateSectionElement(ResourceLoaderDescriptorContainer provider)
        {
            _provider = provider;

            style.flexDirection = FlexDirection.Column;
            style.marginBottom = 10;

            var section = new SectionElement("Create ResourceLoader");
            Add(section);

            var providerElement = new ResourceLoaderDescriptorContainerElement(_provider);
            providerElement.ResourceLoaderDescriptorElement.OnClassNameChanged += EnableButtonIfNecessary;

            _selectedDescriptor = _provider.ActiveDescriptor ?? _provider.Generators.FirstOrDefault();

            _createButton = new Button(CreateResourceLoader) { text = "Create" };
            _createButton.style.marginTop = 10;
            _createButton.SetEnabled(!string.IsNullOrEmpty(_selectedDescriptor.ClassName));

            providerElement.Add(_createButton);
            section.Add(providerElement);
        }

        private void CreateResourceLoader()
        {
            if (string.IsNullOrEmpty(_selectedDescriptor.ClassName))
            {
                EditorUtility.DisplayDialog("Error", "Please enter a class name.", "OK");
                return;
            }

            _selectedDescriptor.Run();
            Debug.Log($"[CreateResourceLoader] Created: {_selectedDescriptor.ClassName}");
        }

        private void EnableButtonIfNecessary()
        {
            _createButton?.SetEnabled(!string.IsNullOrEmpty(_selectedDescriptor?.ClassName));
        }
    }
}
#endif
