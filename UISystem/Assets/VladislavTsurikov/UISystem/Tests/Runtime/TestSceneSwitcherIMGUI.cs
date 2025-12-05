#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;
using VladislavTsurikov.SceneUtility.Editor.Utility;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration;
using Zenject;

namespace VladislavTsurikov.UISystem.Tests.Runtime
{
    public class TestSceneSwitcherIMGUI : MonoBehaviour
    {
        private SceneInstance _currentSceneInstance;

        [Inject]
        private SceneCompositionService _sceneCompositionService;

        private void OnGUI()
        {
            var currentScene = _currentSceneInstance.Scene.name;

            var width = 500;
            var height = 150;
            var spacing = 40;

            var centerX = (Screen.width - width) / 2;
            var centerY = (Screen.height - (height * 2 + spacing)) / 2;

            var style = new GUIStyle(GUI.skin.button);
            style.fontSize = 32;

            if (currentScene != "TestScene_1")
            {
                if (GUI.Button(new Rect(centerX, centerY, width, height), "Go to Scene 1", style))
                {
                    SwitchToScene("TestScene_1").Forget();
                }
            }
            else
            {
                if (GUI.Button(new Rect(centerX, centerY, width, height), "Go to TestBoot", style))
                {
                    SwitchToScene("TestBoot").Forget();
                }
            }

            if (currentScene != "TestScene_2")
            {
                if (GUI.Button(new Rect(centerX, centerY + height + spacing, width, height), "Go to Scene 2", style))
                {
                    SwitchToScene("TestScene_2").Forget();
                }
            }
            else
            {
                if (GUI.Button(new Rect(centerX, centerY + height + spacing, width, height), "Go to TestBoot", style))
                {
                    SwitchToScene("TestBoot").Forget();
                }
            }
        }

        private async UniTask SwitchToScene(string sceneName)
        {
            if (BuildSceneUtility.IsSceneInBuildSettings(sceneName))
            {
                await _sceneCompositionService.LoadBuiltScene(sceneName,
                    async () => {
                        Debug.Log("Built scene loaded.");
                        await UniTask.CompletedTask;
                    });
            }
            else
            {
                _currentSceneInstance = await _sceneCompositionService.LoadAddressableScene(sceneName,
                    async handle => { await handle.ActivateAsync().ToUniTask(); });
            }
            await UniTask.CompletedTask;
        }
    }
}
#endif
