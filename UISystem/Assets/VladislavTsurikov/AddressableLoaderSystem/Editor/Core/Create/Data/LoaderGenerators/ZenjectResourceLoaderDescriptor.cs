#if UNITY_EDITOR
using System;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.ZenjectIntegration;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create
{
    public class ZenjectResourceLoaderDescriptor : ResourceLoaderFieldDescriptor
    {
        public override Type BaseType => typeof(ZenjectResourceLoader);

        public override void Run()
        {
        }
    }
}
#endif
