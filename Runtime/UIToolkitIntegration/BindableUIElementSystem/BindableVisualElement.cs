using UnityEngine.UIElements;
using VladislavTsurikov.UISystem.Runtime.Core;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public abstract class BindableVisualElement : VisualElement, IBindableView
    {
        protected BindableVisualElement() => RegisterCallback<AttachToPanelEvent>(HandleAttachToPanel);

        protected virtual void InitializeElements()
        {
        }

        private void HandleAttachToPanel(AttachToPanelEvent evt)
        {
            InitializeElements();
            UnregisterCallback<AttachToPanelEvent>(HandleAttachToPanel);
        }
    }
}
