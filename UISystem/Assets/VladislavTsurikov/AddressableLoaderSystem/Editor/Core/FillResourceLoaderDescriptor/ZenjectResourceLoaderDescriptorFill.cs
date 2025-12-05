using System;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.ZenjectIntegration;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create
{
    public class ZenjectResourceLoaderDescriptorFill : ResourceLoaderFieldDescriptorFill
    {
        public override Type LoaderType => typeof(ZenjectResourceLoader);
        public override Type LoaderDescriptorBaseType => typeof(ZenjectResourceLoaderDescriptor);
    }
}
