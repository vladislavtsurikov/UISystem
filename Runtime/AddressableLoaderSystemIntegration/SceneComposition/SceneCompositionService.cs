#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.ZenjectUtility.Runtime;
using Zenject;

namespace VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration
{
    public class SceneCompositionService
    {
        protected readonly ResourceLoaderManager _resourceLoaderManager;
        protected readonly ZenjectAddressableSceneLoader _sceneLoader;
        protected readonly SceneUICompositionService _sceneUICompositionService;

        public SceneCompositionService(
            ResourceLoaderManager resourceLoaderManager,
            ZenjectAddressableSceneLoader sceneLoader,
            SceneUICompositionService sceneUICompositionService)
        {
            _resourceLoaderManager = resourceLoaderManager;
            _sceneLoader = sceneLoader;
            _sceneUICompositionService = sceneUICompositionService;
        }

        protected virtual bool IsFilterMatch(FilterAttribute attr, string sceneName)
        {
            return attr is SceneFilterAttribute s && s.Matches(sceneName);
        }

        protected virtual void ExtraBindingsLate(DiContainer container)
        {
        }

        public async UniTask LoadBuiltScene(
            string sceneName,
            Func<UniTask> eventAfterLoadScene = null,
            LoadSceneMode loadSceneMode = LoadSceneMode.Single,
            CancellationToken cancellationToken = default)
        {
            _sceneUICompositionService.RemoveScenePresenters();

            await _resourceLoaderManager.Load(Filter, cancellationToken);

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

            await _sceneUICompositionService.ComposeScene(sceneName, cancellationToken);
            return;

            bool Filter(FilterAttribute attr) => IsFilterMatch(attr, sceneName);
        }

        public async UniTask<SceneInstance> LoadAddressableScene(
            string sceneName,
            Func<SceneInstance, UniTask> eventAfterLoadScene = null,
            Action<DiContainer> extraBindings = null,
            LoadSceneMode loadSceneMode = LoadSceneMode.Single,
            LoadSceneRelationship containerMode = LoadSceneRelationship.None,
            CancellationToken cancellationToken = default)
        {
            _sceneUICompositionService.RemoveScenePresenters();

            await _resourceLoaderManager.Load(Filter, cancellationToken);

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
            await _sceneUICompositionService.ComposeScene(sceneName, cancellationToken);

            return handle;

            bool Filter(FilterAttribute attr) => IsFilterMatch(attr, sceneName);
        }
    }
}
#endif
