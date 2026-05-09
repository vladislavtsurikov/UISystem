using System.Collections.Generic;
using UnityEngine.UIElements;
using VladislavTsurikov.UISystem.Runtime.Core;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public sealed class UIToolkitElementBinder : ViewBindingScope
    {
        public UIToolkitElementBinder(UIPresenter presenter)
            : base(presenter)
        {
        }

        public void BindElementsFrom(VisualElement root)
        {
            if (root == null)
            {
                return;
            }

            RegisterBindings(
                EnumerateElements(root),
                GetBindingId,
                _ => UIPresenter.InstanceKey);
        }

        private static IEnumerable<VisualElement> EnumerateElements(VisualElement root)
        {
            yield return root;

            List<VisualElement> elements = root.Query<VisualElement>().ToList();
            foreach (VisualElement element in elements)
            {
                if (!ReferenceEquals(element, root))
                {
                    yield return element;
                }
            }
        }

        private static string GetBindingId(VisualElement element)
        {
            if (element is IBindableView bindableElement && !string.IsNullOrEmpty(bindableElement.BindingId))
            {
                return bindableElement.BindingId;
            }

            return element.name;
        }
    }
}
