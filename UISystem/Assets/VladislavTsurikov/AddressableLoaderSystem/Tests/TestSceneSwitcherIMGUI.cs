#if ADDRESSABLE_LOADER_SYSTEM_ADDRESSABLES
#if ADDRESSABLE_LOADER_SYSTEM_ZENJECT
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core.Behavior;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Behavior;
using Zenject;

namespace VladislavTsurikov.AddressableLoaderSystem.Tests
{
    public class TestSceneSwitcherIMGUI : MonoBehaviour
    {
        [Inject]
        private SceneBehavior _sceneBehavior;

        private readonly LoaderBehaviorBatch _loaderBehaviorBatch = new();
        private string _currentSceneContext;

        private void Start()
        {
            _currentSceneContext = SceneManager.GetActiveScene().name;
        }

        private void OnGUI()
        {
            var currentScene = SceneManager.GetActiveScene().name;

            var width = 500;
            var height = 150;
            var spacing = 40;

            var centerX = (Screen.width - width) / 2;
            var centerY = (Screen.height - (height * 2 + spacing)) / 2;

            var style = new GUIStyle(GUI.skin.button);
            style.fontSize = 32;

            if (currentScene != "TestScene_A")
            {
                if (GUI.Button(new Rect(centerX, centerY, width, height), "Go to Scene A", style))
                {
                    LoadSceneWithFilters("TestScene_A").Forget();
                }
            }
            else
            {
                if (GUI.Button(new Rect(centerX, centerY, width, height), "Go to TestBoot", style))
                {
                    LoadSceneWithFilters("TestBoot").Forget();
                }
            }

            if (currentScene != "TestScene_B")
            {
                if (GUI.Button(new Rect(centerX, centerY + height + spacing, width, height), "Go to Scene B", style))
                {
                    LoadSceneWithFilters("TestScene_B").Forget();
                }
            }
            else
            {
                if (GUI.Button(new Rect(centerX, centerY + height + spacing, width, height), "Go to TestBoot", style))
                {
                    LoadSceneWithFilters("TestBoot").Forget();
                }
            }
        }

        private async UniTask LoadSceneWithFilters(string sceneName)
        {
            if (_sceneBehavior == null)
            {
                Debug.LogError("[TestSceneSwitcherIMGUI] SceneBehavior is not resolved.");
                return;
            }

            if (!string.IsNullOrEmpty(_currentSceneContext))
            {
                if (_currentSceneContext == sceneName)
                {
                    return;
                }

                _loaderBehaviorBatch.Unload(_sceneBehavior, _currentSceneContext);
            }

            _loaderBehaviorBatch.Load(_sceneBehavior, sceneName);

            await _loaderBehaviorBatch.Run(this.GetCancellationTokenOnDestroy());

            _currentSceneContext = sceneName;

            SceneManager.LoadScene(sceneName);
        }
    }
}
#endif
#endif
