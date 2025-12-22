#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core.Behavior;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Behavior;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.ZenjectUtility.Runtime;
using Zenject;

namespace VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration
{
    public class SceneCompositionService
    {
        protected readonly UIHandlerManager _handlerManager;
        protected readonly LoaderBehaviorBatch _loaderBehaviorBatch = new();
        protected readonly SceneBehavior _sceneBehavior;
        protected readonly ZenjectAddressableSceneLoader _sceneLoader;
        private string _currentSceneContext;

        public SceneCompositionService(
            UIHandlerManager handlerManager,
            SceneBehavior sceneBehavior,
            ZenjectAddressableSceneLoader sceneLoader)
        {
            _handlerManager = handlerManager;
            _sceneBehavior = sceneBehavior;
            _sceneLoader = sceneLoader;
        }

        protected virtual bool IsFilterMatch(FilterAttribute attr, string sceneName) =>
            attr is SceneFilterAttribute s && s.Matches(sceneName);

        protected virtual void ExtraBindingsLate(DiContainer container)
        {
        }

        public async UniTask LoadBuiltScene(
            string sceneName,
            Func<UniTask> eventAfterLoadScene = null,
            LoadSceneMode loadSceneMode = LoadSceneMode.Single,
            CancellationToken cancellationToken = default)
        {
            _handlerManager.RemoveExceptGlobalHandlers();

            await UpdateSceneBehaviors(sceneName, cancellationToken);

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
            if (asyncOperation == null)
            {
                Debug.LogError($"[SceneCompositionService] Failed to load built scene '{sceneName}'");
                return;
            }

            await asyncOperation.ToUniTask(cancellationToken: cancellationToken);

            if (eventAfterLoadScene != null)
            {
                await eventAfterLoadScene();
            }

            await _handlerManager.AddFilter(Filter, cancellationToken);
            return;

            bool Filter(FilterAttribute attr)
            {
                return IsFilterMatch(attr, sceneName);
            }
        }

        public async UniTask<SceneInstance> LoadAddressableScene(
            string sceneName,
            Func<SceneInstance, UniTask> eventAfterLoadScene = null,
            Action<DiContainer> extraBindings = null,
            LoadSceneMode loadSceneMode = LoadSceneMode.Single,
            LoadSceneRelationship containerMode = LoadSceneRelationship.None,
            CancellationToken cancellationToken = default)
        {
            _handlerManager.RemoveExceptGlobalHandlers();

            await UpdateSceneBehaviors(sceneName, cancellationToken);

            SceneInstance handle = await _sceneLoader.LoadSceneAsync(
                sceneName,
                loadSceneMode,
                extraBindings,
                containerMode,
                ExtraBindingsLate);

            if (eventAfterLoadScene != null)
            {
                await eventAfterLoadScene(handle);
            }

            await handle.ActivateAsync().ToUniTask(cancellationToken: cancellationToken);
            await _handlerManager.AddFilter(Filter, cancellationToken);

            return handle;

            bool Filter(FilterAttribute attr)
            {
                return IsFilterMatch(attr, sceneName);
            }
        }

        private async UniTask UpdateSceneBehaviors(string sceneName, CancellationToken cancellationToken)
        {
            if (_sceneBehavior == null)
            {
                Debug.LogError("[SceneCompositionService] SceneBehavior is not resolved.");
                return;
            }

            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("[SceneCompositionService] Scene name is null or empty.");
                return;
            }

            if (sceneName == _currentSceneContext)
            {
                return;
            }

            if (!string.IsNullOrEmpty(_currentSceneContext))
            {
                _loaderBehaviorBatch.Unload(_sceneBehavior, _currentSceneContext);
            }

            _loaderBehaviorBatch.Load(_sceneBehavior, sceneName);

            await _loaderBehaviorBatch.Run(cancellationToken);

            _currentSceneContext = sceneName;
        }
    }
}
#endif
