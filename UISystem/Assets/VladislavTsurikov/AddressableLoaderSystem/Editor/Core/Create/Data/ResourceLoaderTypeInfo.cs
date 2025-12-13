#if UNITY_EDITOR
using System;
using VladislavTsurikov.Utility.Runtime.Extensions;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create
{
    public class ResourceLoaderTypeInfo
    {
        public Type LoaderType { get; }
        public ResourceLoaderDescriptorContainer LoaderDescriptorContainer { get; }
        public ResourceLoaderTemplate ActiveTemplate => LoaderDescriptorContainer.ActiveTemplate;

        public string Name => ActiveTemplate?.ClassName ?? LoaderType?.Name ?? "Unknown";

        public string CsFilePath { get; }

        public ResourceLoaderTypeInfo(Type loaderType, ResourceLoaderDescriptorContainer container)
        {
            LoaderType = loaderType ?? throw new ArgumentNullException(nameof(loaderType));
            LoaderDescriptorContainer = container ?? throw new ArgumentNullException(nameof(container));

            CsFilePath = loaderType.GetSourceFilePath();
        }

        public override string ToString()
        {
            return $"{Name} (Base: {LoaderDescriptorContainer.ActiveType?.Name ?? "None"}) @ {CsFilePath}";
        }
    }
}
#endif
