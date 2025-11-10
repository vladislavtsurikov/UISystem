using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VladislavTsurikov.SceneUtility.Runtime
{
    public class SceneManagerSceneOperations : SceneOperations
    {
        private AsyncOperation _loadSceneAsyncOperation;
        private AsyncOperation _unloadSceneAsyncOperation;

        public SceneManagerSceneOperations(SceneReference sceneReference) : base(sceneReference)
        {
        }

        public override float LoadingProgress()
        {
            if (_loadSceneAsyncOperation != null)
            {
                return _loadSceneAsyncOperation.progress;
            }

            return SceneReference.Scene.isLoaded ? 1 : 0;
        }

        protected override async UniTask LoadScene()
        {
            _loadSceneAsyncOperation = SceneManager.LoadSceneAsync(SceneReference.SceneName, LoadSceneMode.Additive);

            await _loadSceneAsyncOperation;
        }

        protected override async UniTask UnloadScene()
        {
            _loadSceneAsyncOperation = null;

            _unloadSceneAsyncOperation = SceneManager.UnloadSceneAsync(SceneReference.SceneName);

            await _unloadSceneAsyncOperation;
        }

        public override bool IsLoading()
        {
            if (_loadSceneAsyncOperation == null)
            {
                return false;
            }

            return _loadSceneAsyncOperation.progress != 1;
        }

        public override bool IsUnloading()
        {
            if (_unloadSceneAsyncOperation == null)
            {
                return false;
            }

            return _unloadSceneAsyncOperation.progress != 1;
        }
    }
}
