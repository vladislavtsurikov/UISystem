using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core.Behavior
{
    public abstract class LoaderBehavior
    {
        protected internal readonly List<ResourceLoader> Loaders;

        protected LoaderBehavior(List<ResourceLoader> loaders)
        {
            Loaders = loaders;
        }

        public abstract UniTask Load(string context, CancellationToken token);
        public abstract UniTask Unload(string context, CancellationToken token);
    }
}
