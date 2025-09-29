using System;
using Cysharp.Threading.Tasks;
using ModestTree;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Zenject;

namespace Plugins.Zenject.Source.Util
{
    public class ZenjectAddressableSceneLoader
    {
        readonly ProjectKernel _projectKernel;
        readonly DiContainer _sceneContainer;

        public ZenjectAddressableSceneLoader(
            [InjectOptional] SceneContext sceneRoot,
            ProjectKernel projectKernel)
        {
            _projectKernel = projectKernel;
            _sceneContainer = sceneRoot == null ? null : sceneRoot.Container;
        }

        public async UniTask<SceneInstance> LoadSceneAsync(
            string sceneAddress,
            LoadSceneMode loadMode = LoadSceneMode.Single,
            Action<DiContainer> extraBindings = null,
            LoadSceneRelationship containerMode = LoadSceneRelationship.None,
            Action<DiContainer> extraBindingsLate = null,
            bool activateOnLoad = false)
        {
            PrepareForLoadScene(loadMode, extraBindings, extraBindingsLate, containerMode);

            var handle = Addressables.LoadSceneAsync(sceneAddress, loadMode, activateOnLoad);
            await handle.ToUniTask();
            return handle.Result;
        }

        private void PrepareForLoadScene(
            LoadSceneMode loadMode,
            Action<DiContainer> extraBindings,
            Action<DiContainer> extraBindingsLate,
            LoadSceneRelationship containerMode)
        {
            if (loadMode == LoadSceneMode.Single)
            {
                Assert.IsEqual(containerMode, LoadSceneRelationship.None);
                _projectKernel.ForceUnloadAllScenes();
            }

            if (containerMode == LoadSceneRelationship.None)
            {
                SceneContext.ParentContainers = null;
            }
            else if (containerMode == LoadSceneRelationship.Child)
            {
                SceneContext.ParentContainers = _sceneContainer == null
                    ? null
                    : new[] { _sceneContainer };
            }
            else
            {
                Assert.IsNotNull(_sceneContainer, "Cannot use LoadSceneRelationship.Sibling when loading from ProjectContext");
                SceneContext.ParentContainers = _sceneContainer.ParentContainers;
            }

            SceneContext.ExtraBindingsInstallMethod = extraBindings;
            SceneContext.ExtraBindingsLateInstallMethod = extraBindingsLate;
        }
    }
}