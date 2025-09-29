using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using Zenject;

namespace VladislavTsurikov.AddressableLoaderSystem.Tests
{
    public class TestSceneSwitcherIMGUI : MonoBehaviour
    {
        [Inject]
        private ResourceLoaderManager _resourceLoaderManager;

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
            await _resourceLoaderManager.Load(attr =>
                attr is SceneFilterAttribute s && s.Matches(sceneName));

            SceneManager.LoadScene(sceneName);
        }
    }
}
