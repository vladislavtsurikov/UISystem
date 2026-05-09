#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.Core;

namespace VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration
{
    public sealed class SceneUICompositionService
    {
        private readonly UIPresenterManager _presenterManager;

        public SceneUICompositionService(UIPresenterManager presenterManager) => _presenterManager = presenterManager;

        public void RemoveScenePresenters()
        {
            _presenterManager.RemoveExceptGlobalPresenters();
        }

        public UniTask ComposeScene(string sceneName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                return UniTask.CompletedTask;
            }

            return _presenterManager.AddFilter(
                attribute => attribute is SceneFilterAttribute sceneFilter && sceneFilter.Matches(sceneName),
                cancellationToken);
        }

        public UniTask RecomposeScene(string sceneName, CancellationToken cancellationToken = default)
        {
            RemoveScenePresenters();
            return ComposeScene(sceneName, cancellationToken);
        }
    }
}
#endif
