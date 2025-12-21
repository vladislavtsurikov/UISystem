using VladislavTsurikov.Core.Editor;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    [ElementEditor(typeof(ZenjectResourceLoaderTemplate))]
    public class ZenjectResourceLoaderTemplateElement : ResourceLoaderFieldTemplateElement
    {
        public ZenjectResourceLoaderTemplateElement(ResourceLoaderTemplate template) : base(template)
        {

        }
    }
}
