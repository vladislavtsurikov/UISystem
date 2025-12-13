#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    public class SearchSectionElement : VisualElement
    {
        private TextField _searchField;
        private VisualElement _resultsContainer;
        private ResourceLoaderTypeInfo _selectedLoader;

        public SearchSectionElement()
        {
            style.flexDirection = FlexDirection.Column;
            style.marginBottom = 10;

            var section = new SectionElement("Search ResourceLoader");
            Add(section);

            _searchField = new TextField();
            _searchField.style.marginBottom = 8;
            _searchField.RegisterValueChangedCallback(evt =>
            {
                var query = evt.newValue?.Trim();
                _selectedLoader = string.IsNullOrEmpty(query)
                    ? null
                    : EditorResourceLoaderRegistry.Wrappers.FirstOrDefault(x =>
                        x.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0);
                RefreshSearchResults(query);
            });
            section.Add(_searchField);

            _resultsContainer = new VisualElement();
            _resultsContainer.style.flexDirection = FlexDirection.Column;
            _resultsContainer.style.marginTop = 4;
            section.Add(_resultsContainer);
        }

        public void RefreshSearchResults(string query)
        {
            _resultsContainer.Clear();

            var type = ResourceLoaderTypeRegistry.GetTypeByName(query);

            if (type != null && _selectedLoader == null)
            {
                var errorLabel = new Label($"No ResourceLoaderDescriptor found for {query}");
                errorLabel.style.color = new Color(1f, 0.5f, 0.5f);
                _resultsContainer.Add(errorLabel);
                return;
            }

            if (string.IsNullOrEmpty(query))
            {
                var info = new Label("Type a ResourceLoader name to display it");
                info.style.color = new Color(0.7f, 0.7f, 0.7f);
                info.style.marginTop = 10;
                _resultsContainer.Add(info);
                return;
            }

            if (_selectedLoader != null)
            {
                _resultsContainer.Add(new LoaderSectionElement(_selectedLoader));
            }
        }
    }
}
#endif
