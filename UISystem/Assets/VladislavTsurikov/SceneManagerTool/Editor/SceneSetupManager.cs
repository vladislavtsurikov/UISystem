#if UNITY_EDITOR
using System;
using System.Linq;
using OdinSerializer;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace VladislavTsurikov.SceneManagerTool.Editor
{
    [Serializable]
    internal class SceneSetupManager
    {
        [OdinSerialize]
        internal SceneSetup[] SceneSetups;

        internal void Setup()
        {
            EditorApplication.playModeStateChanged -= EditorApplicationPlayModeStateChanged;
            EditorApplication.playModeStateChanged += EditorApplicationPlayModeStateChanged;

            void EditorApplicationPlayModeStateChanged(PlayModeStateChange state)
            {
                if (state == PlayModeStateChange.EnteredEditMode)
                {
                    RestoreSceneSetup();
                }
            }
        }

        internal void SaveSceneSetup()
        {
            if (EditorSceneManager.GetSceneManagerSetup().Any())
            {
                SceneSetups = EditorSceneManager.GetSceneManagerSetup();
            }
        }

        private void RestoreSceneSetup()
        {
            if (SceneSetups == null || !SceneSetups.Any())
            {
                return;
            }

            EditorSceneManager.RestoreSceneManagerSetup(SceneSetups);

            SceneSetups = Array.Empty<SceneSetup>();
        }
    }
}
#endif
