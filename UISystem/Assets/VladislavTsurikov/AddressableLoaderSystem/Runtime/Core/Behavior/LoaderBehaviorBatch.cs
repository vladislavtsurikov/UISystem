using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core.Behavior
{
    public class LoaderBehaviorBatch
    {
        private readonly List<(LoaderBehavior behavior, string context, bool load)> _operations = new();

        public LoaderBehaviorBatch Load(LoaderBehavior behavior, string context)
        {
            _operations.Add((behavior, context, true));
            return this;
        }

        public LoaderBehaviorBatch Unload(LoaderBehavior behavior, string context)
        {
            _operations.Add((behavior, context, false));
            return this;
        }

        public async UniTask Run(CancellationToken token)
        {
            foreach ((LoaderBehavior behavior, var context, var load) in _operations)
            {
                if (load)
                {
                    await behavior.Load(context, token);
                }
                else
                {
                    await behavior.Unload(context, token);
                }
            }

            await ActiveResourceLoaderRegistry.Run(token);
            _operations.Clear();
        }
    }
}
