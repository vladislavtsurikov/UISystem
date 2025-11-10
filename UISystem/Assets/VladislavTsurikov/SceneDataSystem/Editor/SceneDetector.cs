#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.SceneDataSystem.Editor
{
    [InitializeOnLoad]
    public static class SceneDetector
    {
        static SceneDetector()
        {
            SetupSceneDataManager();

            SceneManagement.SceneLoadedOrUnloaded -= SetupSceneDataManager;
            SceneManagement.SceneLoadedOrUnloaded += SetupSceneDataManager;
        }

        private static void SetupSceneDataManager()
        {
            SceneDataManagerUtility.InstanceSceneDataManagerForAllScenes();
            RequiredSceneData.CreateAllRequiredTypes();
        }
    }
}
#endif
