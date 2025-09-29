#if SCENE_MANAGER_ADDRESSABLES
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using VladislavTsurikov.SceneUtility.Runtime;
using VladislavTsurikov.Utility.Runtime.Extensions;
using UniTask = Cysharp.Threading.Tasks.UniTask;

namespace VladislavTsurikov.SceneUtility.Scripts.Integration.Addressables
{
    public class AddressablesSceneOperations : SceneOperations
    {
        private AsyncOperationHandle<SceneInstance> _loadSceneAsyncOperation;
        private AsyncOperationHandle<SceneInstance> _unloadSceneAsyncOperation;

        public AddressablesSceneOperations(SceneReference sceneReference) : base(sceneReference)
        {
        }

        public override float LoadingProgress()
        {
            if (_loadSceneAsyncOperation.IsValid())
            {
                return _loadSceneAsyncOperation.PercentComplete;
            }

            return SceneReference.Scene.isLoaded ? 1 : 0;
        }

        protected override async UniTask LoadScene()
        {
            _loadSceneAsyncOperation =
                UnityEngine.AddressableAssets.Addressables.LoadSceneAsync(SceneReference.ScenePath,
                    LoadSceneMode.Additive);
            await _loadSceneAsyncOperation;
        }

        protected override async UniTask UnloadScene()
        {
            if (!_loadSceneAsyncOperation.IsValid())
            {
                return;
            }

            _unloadSceneAsyncOperation =
                UnityEngine.AddressableAssets.Addressables.UnloadSceneAsync(_loadSceneAsyncOperation);
            await _unloadSceneAsyncOperation;
        }

        public override bool IsLoading()
        {
            if (!_loadSceneAsyncOperation.IsValid())
            {
                return false;
            }

            return _loadSceneAsyncOperation.PercentComplete != 1;
        }

        public override bool IsUnloading()
        {
            if (!_unloadSceneAsyncOperation.IsValid())
            {
                return false;
            }

            return _unloadSceneAsyncOperation.PercentComplete != 1;
        }
    }
}
#endif
