#if UNITY_EDITOR
using UnityEngine.UIElements;
using VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create.ResourceLoaderDescriptorElements;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create
{
    public sealed class ResourceLoaderTemplateElement : VisualElement
    {
        private ResourceLoaderTemplate _template;
        private ResourceLoaderDescriptorElement _descriptorElement;
        private readonly VisualElement _descriptorRoot;

        public ResourceLoaderTemplate Template => _template;

        public ResourceLoaderTemplateElement()
        {
            _descriptorRoot = new VisualElement();
            _descriptorRoot.name = "ResourceLoaderDescriptorRoot";

            Add(_descriptorRoot);
        }

        public void SetTemplate(ResourceLoaderTemplate template)
        {
            _template = template;
            Refresh();
        }

        public void Refresh()
        {
            _descriptorRoot.Clear();

            _descriptorElement = ResourceLoaderDescriptorEditorStack.GetElement(_template);
            if (_descriptorElement == null)
            {
                return;
            }

            _descriptorRoot.Add(_descriptorElement);
            _descriptorElement.RefreshForm();
        }
    }
}
#endif
