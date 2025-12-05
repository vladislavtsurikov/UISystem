using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core.Behavior;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Behavior
{
    public class SceneBehavior : LoaderBehavior
    {
        public SceneBehavior(List<ResourceLoader> loaders)
            : base(loaders)
        {
        }

        public override UniTask Load(string sceneName, CancellationToken token)
        {
            foreach (ResourceLoader loader in GetSceneLoaders(sceneName))
            {
                ActiveResourceLoaderRegistry.RequestLoad(loader);
            }

            return UniTask.CompletedTask;
        }

        public override UniTask Unload(string sceneName, CancellationToken token)
        {
            foreach (ResourceLoader loader in GetSceneLoaders(sceneName))
            {
                ActiveResourceLoaderRegistry.RequestUnload(loader);
            }

            return UniTask.CompletedTask;
        }

        private IEnumerable<ResourceLoader> GetSceneLoaders(string sceneName)
        {
            foreach (ResourceLoader loader in Loaders)
            {
                BehaviorAttribute attr = loader.GetType()
                    .GetCustomAttributes(typeof(BehaviorAttribute), false)
                    .Cast<BehaviorAttribute>()
                    .FirstOrDefault();

                if (attr == null)
                {
                    continue;
                }

                if (attr.BehaviorType == typeof(SceneBehavior) && attr.Contexts.Contains(sceneName))
                {
                    yield return loader;
                }
            }
        }
    }
}
