using System;
using VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    public abstract class ResourceLoaderDescriptorFill
    {
        public abstract Type LoaderType { get; }
        public abstract Type LoaderDescriptorBaseType { get; }

        public abstract ResourceLoaderDescriptor Fill(Type loaderType);

        protected ResourceLoaderDescriptor InstanceResourceLoaderDescriptor()
        {
            return Activator.CreateInstance(LoaderDescriptorBaseType) as ResourceLoaderDescriptor;
        }
    }
}
