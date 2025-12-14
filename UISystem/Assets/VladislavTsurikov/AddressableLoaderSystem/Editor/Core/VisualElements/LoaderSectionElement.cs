#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    public sealed class LoaderSectionElement : VisualElement
    {
        private readonly Button _refreshButton;
        private readonly ResourceLoaderTemplateEditorStackElement _templateEditorStackElement;

        private ResourceLoaderTemplate _selectedTemplate;

        public LoaderSectionElement(ResourceLoaderTemplate template)
        {
            _selectedTemplate = template;

            style.flexDirection = FlexDirection.Column;
            style.marginBottom = 10;

            if (!string.IsNullOrEmpty(_selectedTemplate.CsFilePath))
            {
                Button openBtn = new Button(OpenScript)
                {
                    text = "Open Script"
                };

                openBtn.style.marginBottom = 8;
                Add(openBtn);
            }

            _templateEditorStackElement = new ResourceLoaderTemplateEditorStackElement();
            _templateEditorStackElement.SetTemplate(_selectedTemplate);
            Add(_templateEditorStackElement);

            _refreshButton = new Button(RunGenerator)
            {
                text = "Refresh",
            };
            _refreshButton.style.marginTop = 10;
            _refreshButton.SetEnabled(false);
            Add(_refreshButton);

            RegisterCallback<ChangeEvent<string>>(_ => EnableRefresh());
            RegisterCallback<ChangeEvent<bool>>(_ => EnableRefresh());
        }

        private void OpenScript()
        {
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(_selectedTemplate.CsFilePath);
            if (asset != null)
            {
                AssetDatabase.OpenAsset(asset);
            }
        }

        private void EnableRefresh()
        {
            _refreshButton.SetEnabled(true);
        }

        private void RunGenerator()
        {
            if (_selectedTemplate == null)
            {
                Debug.LogWarning("[LoaderSection] No selected template found to refresh.");
                return;
            }

            _selectedTemplate.Run();
            Debug.Log($"[LoaderSection] Refreshed: {_selectedTemplate.ClassName}");

            _refreshButton.SetEnabled(false);
        }
    }
}
#endif
