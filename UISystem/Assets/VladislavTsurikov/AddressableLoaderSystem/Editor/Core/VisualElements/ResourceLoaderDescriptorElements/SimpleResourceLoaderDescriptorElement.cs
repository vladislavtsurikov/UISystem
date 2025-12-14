namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    [LoaderBaseType(typeof(SimpleResourceLoaderTemplate))]
    public class SimpleResourceLoaderDescriptorElement : ResourceLoaderFieldDescriptorElement
    {
        public SimpleResourceLoaderDescriptorElement(ResourceLoaderTemplate template) : base(template)
        {
        }
    }
}
