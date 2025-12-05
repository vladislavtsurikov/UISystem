#if UNITY_EDITOR
using System;
using OdinSerializer;
using UnityEditor;
using UnityEditor.SceneManagement;
using VladislavTsurikov.SceneManagerTool.Runtime;
using VladislavTsurikov.SceneUtility.Editor;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Editor
{
    [Serializable]
    public sealed class StartupScene
    {
        [OdinSerialize]
        private SceneReference _sceneReference = new();

        public void Setup()
        {
            if (_sceneReference.IsValid())
            {
                return;
            }

            EditorApplication.delayCall += () =>
            {
                _sceneReference = new SceneReference(SceneCreationUtility.CreateScene("Scene Manager",
                    SceneManagerPath.PathToResourcesSceneManager));
            };
        }

        public static void Open() => EditorSceneManager.OpenScene(GetStartupScenePath(), OpenSceneMode.Single);

        public static string GetStartupScenePath()
        {
            if (!SceneManagerData.Instance.SceneManagerEditorData.StartupScene._sceneReference.IsValid())
            {
                SceneManagerData.Instance.SceneManagerEditorData.StartupScene.Setup();
            }

            return SceneManagerData.Instance.SceneManagerEditorData.StartupScene._sceneReference.ScenePath;
        }
    }
}
#endif
