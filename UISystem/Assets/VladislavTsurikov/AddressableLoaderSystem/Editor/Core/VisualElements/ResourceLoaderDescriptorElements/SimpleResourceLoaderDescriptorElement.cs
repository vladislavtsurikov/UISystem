using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create.ResourceLoaderDescriptorElements
{
    [LoaderBaseType(typeof(SimpleResourceLoaderTemplate))]
    public class SimpleResourceLoaderDescriptorElement : ResourceLoaderFieldDescriptorElement
    {
        public SimpleResourceLoaderDescriptorElement(ResourceLoaderTemplate template) : base(template)
        {
        }
    }
}
