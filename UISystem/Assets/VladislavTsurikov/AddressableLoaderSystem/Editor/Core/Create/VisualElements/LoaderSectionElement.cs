#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    public sealed class LoaderSectionElement : VisualElement
    {
        private readonly Button _refreshButton;
        private readonly ResourceLoaderTypeInfo _typeInfo;

        public LoaderSectionElement(ResourceLoaderTypeInfo typeInfo)
        {
            _typeInfo = typeInfo;

            style.flexDirection = FlexDirection.Column;
            style.marginBottom = 10;

            if (!string.IsNullOrEmpty(typeInfo.CsFilePath))
            {
                var openBtn = new Button(() =>
                {
                    var asset = AssetDatabase.LoadAssetAtPath<Object>(typeInfo.CsFilePath);
                    if (asset != null)
                        AssetDatabase.OpenAsset(asset);
                })
                { text = "Open Script" };

                openBtn.style.marginBottom = 8;
                Add(openBtn);
            }

            var providerElement = new ResourceLoaderDescriptorContainerElement(typeInfo.LoaderDescriptorContainer);
            Add(providerElement);

            _refreshButton = new Button(RunGenerator)
            {
                text = "Refresh",
            };
            _refreshButton.style.marginTop = 10;
            _refreshButton.SetEnabled(false);
            Add(_refreshButton);

            providerElement.RegisterCallback<ChangeEvent<string>>(_ => EnableRefresh());
            providerElement.RegisterCallback<ChangeEvent<bool>>(_ => EnableRefresh());
        }

        private void EnableRefresh()
        {
            _refreshButton.SetEnabled(true);
        }

        private void RunGenerator()
        {
            var generator = _typeInfo.LoaderDescriptorContainer.ActiveDescriptor;

            if (generator == null)
            {
                Debug.LogWarning("[LoaderSection] No active generator found to refresh.");
                return;
            }

            generator.Run();
            Debug.Log($"[LoaderSection] Refreshed: {generator.ClassName}");

            _refreshButton.SetEnabled(false);
        }
    }
}
#endif
