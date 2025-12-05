#if UNITY_EDITOR
using System;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using VladislavTsurikov.SceneManagerTool.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Editor
{
    public static class SceneManagerEditorUtility
    {
        public static void SetAllScenesToDirty()
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                EditorSceneManager.MarkSceneDirty(scene);
            }
        }

        public static void EnterPlaymode()
        {
            SceneManagerData.MaskAsDirty();

            EnterPlaymodeAsync().Forget();

            async UniTask EnterPlaymodeAsync()
            {
                while (EditorApplication.isCompiling)
                {
                    await UniTask.Yield();
                }

                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

                SceneManagerData.Instance.SceneManagerEditorData.SceneSetupManager.SaveSceneSetup();
                StartupScene.Open();

                await UniTask.Delay(TimeSpan.FromSeconds(0.1f), true);

                EditorApplication.EnterPlaymode();
                SceneManagerData.Instance.SceneManagerEditorData.RunAsBuildMode = true;
            }
        }
    }
}
#endif
