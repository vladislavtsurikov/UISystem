#if UNITY_EDITOR
using System;
using VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create.Attributes;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.ZenjectIntegration;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create
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
