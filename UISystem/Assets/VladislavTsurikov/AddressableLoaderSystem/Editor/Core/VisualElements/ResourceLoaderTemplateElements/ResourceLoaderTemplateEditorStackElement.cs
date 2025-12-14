#if UNITY_EDITOR
using UnityEngine.UIElements;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    public sealed class ResourceLoaderTemplateEditorStackElement : VisualElement
    {
        private ResourceLoaderTemplate _template;
        private ResourceLoaderTemplateElement _templateElement;
        private readonly VisualElement _descriptorRoot;

        public ResourceLoaderTemplate Template => _template;

        public ResourceLoaderTemplateEditorStackElement()
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

            _templateElement = ResourceLoaderTemplateEditorStack.GetElement(_template);
            if (_templateElement == null)
            {
                return;
            }

            _descriptorRoot.Add(_templateElement);
            _templateElement.RefreshForm();
        }
    }
}
#endif
