
namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    public abstract class ResourceLoaderFieldDescriptorElement : ResourceLoaderDescriptorElement
    {
        private ResourceLoaderFieldsElement _fieldsElement;

        public ResourceLoaderFieldDescriptorElement(ResourceLoaderTemplate template) : base(template)
        {
        }

        protected override void OnGUI()
        {
            var resourceLoaderFieldDescriptor = (ResourceLoaderFieldTemplate)Template;

            var fieldsBlock = CreateSectionBlock("Fields");
            _fieldsElement = new ResourceLoaderFieldsElement(resourceLoaderFieldDescriptor.Fields);
            fieldsBlock.Add(_fieldsElement);
            _formContainer.Add(fieldsBlock);
        }
    }
}
