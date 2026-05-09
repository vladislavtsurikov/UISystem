#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.UISystem.Runtime.Core;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public sealed class UIToolkitSpawnOperation
        : UISpawnOperation<UIToolkitSpawnOperation, VisualElement, VisualElement, UIToolkitLayoutLoader,
            UIToolkitElementBinder>
    {
        public UIToolkitSpawnOperation Visible(bool visible)
        {
            return SetVisibleState(visible);
        }

        protected override async UniTask<VisualElement> CreateInstance(
            UIToolkitLayoutLoader layoutLoader,
            VisualElement parent,
            CancellationToken cancellationToken)
        {
            if (parent == null)
            {
                Debug.LogError(
                    $"[UIToolkitSpawnOperation] Parent element is null for loader: {layoutLoader.GetType().Name}");
                return null;
            }

            VisualTreeAsset layout = await layoutLoader.LoadLayoutIfNotLoaded(cancellationToken);
            if (layout == null)
            {
                Debug.LogError(
                    $"[UIToolkitSpawnOperation] Layout is null for loader: {layoutLoader.GetType().Name} (Address: {layoutLoader.LayoutAddress})");
                return null;
            }

            TemplateContainer templateContainer = layout.CloneTree();
            return ExtractRootElement(templateContainer, layoutLoader);
        }

        protected override void ApplyName(VisualElement instance, string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                instance.name = name;
            }
        }

        protected override void ApplyVisibility(VisualElement instance, bool visible)
        {
            instance.style.display = visible ? StyleKeyword.Null : DisplayStyle.None;
        }

        protected override void AttachToParent(VisualElement instance, VisualElement parent)
        {
            parent.Add(instance);
        }

        protected override void Bind(VisualElement instance, UIToolkitElementBinder elementBinder)
        {
            elementBinder.BindElementsFrom(instance);
        }

        private static VisualElement ExtractRootElement(
            TemplateContainer templateContainer,
            UIToolkitLayoutLoader layoutLoader)
        {
            if (templateContainer.childCount != 1)
            {
                Debug.LogError(
                    $"[UIToolkitSpawnOperation] Layout `{layoutLoader.LayoutAddress}` must contain exactly one root element.");
                return null;
            }

            VisualElement rootElement = templateContainer.ElementAt(0);
            rootElement.RemoveFromHierarchy();

            for (int i = 0; i < templateContainer.styleSheets.count; i++)
            {
                StyleSheet styleSheet = templateContainer.styleSheets[i];
                rootElement.styleSheets.Add(styleSheet);
            }

            return rootElement;
        }
    }
}
#endif
