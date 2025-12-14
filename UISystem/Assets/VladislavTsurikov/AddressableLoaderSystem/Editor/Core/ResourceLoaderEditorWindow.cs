#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Warning;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    public class ResourceLoaderEditorWindow : EditorWindow
    {
        private VisualElement _warningContainer;
        private SearchSectionElement _searchSection;
        private CreateSectionElement _createSection;
        private ResourceLoaderTemplate _resourceLoaderTemplate;

        [MenuItem("Tools/Addressable Loader/Resource Loader Editor")]
        public static void ShowWindow()
        {
            ResourceLoaderEditorWindow wnd = GetWindow<ResourceLoaderEditorWindow>();
            wnd.titleContent = new GUIContent("Resource Loader Editor");
            wnd.minSize = new Vector2(700, 500);
        }

        private void OnEnable()
        {
            _resourceLoaderTemplate = ResourceLoaderTemplateTypeRegistry.CreateDefaultInstance();
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            root.style.flexDirection = FlexDirection.Column;
            root.style.flexGrow = 1;
            root.style.paddingLeft = 10;
            root.style.paddingTop = 10;

            Button refreshButton = new Button(RefreshRegistry) { text = "Refresh" };
            refreshButton.style.width = 100;
            refreshButton.style.height = 24;
            refreshButton.style.unityFontStyleAndWeight = FontStyle.Bold;
            root.Add(refreshButton);

            ScrollView mainScroll = new ScrollView(ScrollViewMode.Vertical)
            {
                style = { flexGrow = 1 }
            };
            mainScroll.verticalScrollerVisibility = ScrollerVisibility.Auto;
            mainScroll.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            root.Add(mainScroll);

            _warningContainer = new VisualElement();
            _warningContainer.style.marginBottom = 10;
            mainScroll.Add(_warningContainer);

            _searchSection = new SearchSectionElement();
            mainScroll.Add(_searchSection);

            _createSection = new CreateSectionElement(_resourceLoaderTemplate, OnTemplateChanged);
            mainScroll.Add(_createSection);

            RefreshRegistry();
        }

        private void RefreshRegistry()
        {
            EditorResourceLoaderRegistry.Refresh();
            UpdateWarningSection();
        }

        private void UpdateWarningSection()
        {
            _warningContainer.Clear();

            List<AddressableLoaderValidator.ValidationResult> validationResults =
                AddressableLoaderValidator.ValidateAll();

            List<string> invalidLoaders = validationResults
                .Where(r => r.Issues != null && r.Issues.Count > 0 && r.LoaderType != null)
                .Select(r => r.LoaderType.Name)
                .Distinct()
                .ToList();

            if (invalidLoaders.Count == 0)
            {
                return;
            }

            MissingAutoLoadWarningElement warningElement = new MissingAutoLoadWarningElement(invalidLoaders);
            _warningContainer.Add(warningElement);
        }

        private void OnTemplateChanged(ResourceLoaderTemplate template)
        {
            _resourceLoaderTemplate = template;
        }
    }
}
#endif
