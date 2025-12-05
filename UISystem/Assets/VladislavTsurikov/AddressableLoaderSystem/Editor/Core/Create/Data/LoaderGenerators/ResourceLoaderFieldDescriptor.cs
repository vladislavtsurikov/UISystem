
using System.Collections.Generic;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create
{
    public abstract class ResourceLoaderFieldDescriptor : ResourceLoaderDescriptor
    {
        public List<ResourceLoaderFieldData> Fields  = new();
    }
}
