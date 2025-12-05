using System;
using VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    public class SimpleResourceLoaderDescriptorFill : ResourceLoaderFieldDescriptorFill
    {
        public override Type LoaderType => typeof(ResourceLoader);
        public override Type LoaderDescriptorBaseType => typeof(SimpleResourceLoaderDescriptor);
    }
}
