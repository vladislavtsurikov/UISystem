using VladislavTsurikov.Core.Editor;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    [ElementEditor(typeof(SimpleResourceLoaderTemplate))]
    public class SimpleResourceLoaderDescriptorElement : ResourceLoaderFieldDescriptorElement
    {
        public SimpleResourceLoaderDescriptorElement(ResourceLoaderTemplate template) : base(template)
        {
        }
    }
}
