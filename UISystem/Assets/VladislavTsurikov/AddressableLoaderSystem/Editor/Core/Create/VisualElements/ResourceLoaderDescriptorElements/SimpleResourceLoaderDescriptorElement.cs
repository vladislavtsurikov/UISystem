using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create.ResourceLoaderDescriptorElements
{
    [LoaderBaseType(typeof(ResourceLoaderDescriptor))]
    public class SimpleResourceLoaderDescriptorElement : ResourceLoaderFieldDescriptorElement
    {
        public SimpleResourceLoaderDescriptorElement(ResourceLoaderDescriptor descriptor) : base(descriptor)
        {
        }
    }
}
