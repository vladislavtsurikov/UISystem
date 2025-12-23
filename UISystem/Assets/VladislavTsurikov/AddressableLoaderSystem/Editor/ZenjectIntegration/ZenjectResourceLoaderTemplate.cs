#if UNITY_EDITOR
using VladislavTsurikov.AddressableLoaderSystem.Runtime.ZenjectIntegration;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    [ResourceLoaderTemplateBaseType(typeof(ZenjectResourceLoader))]
    public class ZenjectResourceLoaderTemplate : ResourceLoaderFieldTemplate
    {
        public override void Run()
        {
        }
    }
}
#endif
