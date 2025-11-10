#if UNITY_EDITOR
using UnityEngine.SceneManagement;
using VladislavTsurikov.SceneDataSystem.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;

namespace VladislavTsurikov.SceneDataSystem.Editor.Utility
{
    public static class SceneDataSystemUtility
    {
        public static void SetActiveSceneAsParentSceneType()
        {
            Scene activeScene = SceneManager.GetActiveScene();

            SceneDataManager sceneDataManager = SceneDataManagerUtility.InstanceSceneDataManager(activeScene);

            sceneDataManager.SceneType = SceneType.ParentScene;
        }
    }
}
#endif
