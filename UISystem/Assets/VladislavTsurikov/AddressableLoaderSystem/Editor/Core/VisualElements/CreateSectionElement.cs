#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    public sealed class CreateSectionElement : VisualElement
    {
        private ResourceLoaderTemplate _selectedTemplate;
        private readonly Action<ResourceLoaderTemplate> _onTemplateChanged;

        private Button _createButton;
        private ResourceLoaderTemplateChangeElement _templateChangeElement;

        public CreateSectionElement(ResourceLoaderTemplate initialTemplate, Action<ResourceLoaderTemplate> onTemplateChanged)
        {
            _selectedTemplate = initialTemplate;
            _onTemplateChanged = onTemplateChanged;

            style.flexDirection = FlexDirection.Column;
            style.marginBottom = 10;

            SectionElement section = new SectionElement("Create ResourceLoader");
            Add(section);

            _templateChangeElement = new ResourceLoaderTemplateChangeElement(_selectedTemplate);
            _templateChangeElement.TemplateChanged += OnTemplateChanged;
            section.Add(_templateChangeElement);

            _createButton = new Button(CreateResourceLoader) { text = "Create" };
            _createButton.style.marginTop = 10;
            _createButton.SetEnabled(!string.IsNullOrEmpty(_selectedTemplate != null ? _selectedTemplate.ClassName : null));
            section.Add(_createButton);

            _templateChangeElement.RegisterCallback<ChangeEvent<string>>(_ => EnableButtonIfNecessary());
            _templateChangeElement.RegisterCallback<ChangeEvent<bool>>(_ => EnableButtonIfNecessary());

            NotifyWindow();
        }

        private void OnTemplateChanged(ResourceLoaderTemplate template)
        {
            _selectedTemplate = template;

            EnableButtonIfNecessary();
            NotifyWindow();
        }

        private void NotifyWindow()
        {
            _onTemplateChanged?.Invoke(_selectedTemplate);
        }

        private void CreateResourceLoader()
        {
            if (_selectedTemplate == null)
            {
                EditorUtility.DisplayDialog("Error", "Template is not selected.", "OK");
                Debug.LogWarning("[AddressableLoaderSystem][CreateSectionElement.CreateResourceLoader] Template is not selected.");
                return;
            }

            if (string.IsNullOrEmpty(_selectedTemplate.ClassName))
            {
                EditorUtility.DisplayDialog("Error", "Please enter a class name.", "OK");
                Debug.LogWarning("[AddressableLoaderSystem][CreateSectionElement.CreateResourceLoader] Class name is empty.");
                return;
            }

            _selectedTemplate.Run();
            Debug.Log($"[CreateResourceLoader] Created: {_selectedTemplate.ClassName}");
        }

        private void EnableButtonIfNecessary()
        {
            bool enabled = _selectedTemplate != null && !string.IsNullOrEmpty(_selectedTemplate.ClassName);
            _createButton.SetEnabled(enabled);
        }
    }
}
#endif
