using VladislavTsurikov.Core.Editor;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    [ElementEditor(typeof(SimpleResourceLoaderTemplate))]
    public class SimpleResourceLoaderTemplateElement : ResourceLoaderFieldTemplateElement
    {
        public SimpleResourceLoaderTemplateElement(ResourceLoaderTemplate template) : base(template)
        {
        }
    }
}
