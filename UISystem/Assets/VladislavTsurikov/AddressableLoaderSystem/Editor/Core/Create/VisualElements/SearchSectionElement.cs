#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create;

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
                RefreshSearchResults();
            });
            section.Add(_searchField);

            _resultsContainer = new VisualElement();
            _resultsContainer.style.flexDirection = FlexDirection.Column;
            _resultsContainer.style.marginTop = 4;
            section.Add(_resultsContainer);
        }

        public void RefreshSearchResults()
        {
            _resultsContainer.Clear();

            if (_selectedLoader == null)
            {
                var info = new Label("Type a ResourceLoader name to display it");
                info.style.color = new Color(0.7f, 0.7f, 0.7f);
                info.style.marginTop = 10;
                _resultsContainer.Add(info);
                return;
            }

            _resultsContainer.Add(new LoaderSectionElement(_selectedLoader));
        }
    }
}
#endif
