
namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create.ResourceLoaderDescriptorElements
{
    public abstract class ResourceLoaderFieldDescriptorElement : ResourceLoaderDescriptorElement
    {
        private ResourceLoaderFieldsElement _fieldsElement;

        public ResourceLoaderFieldDescriptorElement(ResourceLoaderDescriptor descriptor) : base(descriptor)
        {
        }

        protected override void OnGUI()
        {
            var resourceLoaderFieldDescriptor = (ResourceLoaderFieldDescriptor)_descriptor;

            var fieldsBlock = CreateSectionBlock("Fields");
            _fieldsElement = new ResourceLoaderFieldsElement(resourceLoaderFieldDescriptor.Fields);
            fieldsBlock.Add(_fieldsElement);
            _formContainer.Add(fieldsBlock);
        }
    }
}
