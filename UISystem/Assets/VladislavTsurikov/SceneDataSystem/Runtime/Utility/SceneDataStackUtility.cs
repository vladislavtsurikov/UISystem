using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace VladislavTsurikov.SceneDataSystem.Runtime.Utility
{
    public static class SceneDataStackUtility
    {
        public static T InstanceSceneData<T>(Scene scene) where T : SceneData
        {
            if (!scene.isLoaded)
            {
                return null;
            }

            SceneDataManager sceneDataManager = SceneDataManagerUtility.InstanceSceneDataManager(scene);

            return (T)sceneDataManager.SceneDataStack.CreateIfMissingType(typeof(T));
        }

        public static void Setup<T>(bool force) where T : SceneData
        {
            foreach (SceneDataManager sceneDataManager in SceneDataManagerUtility.GetAllSceneDataManager())
            {
                sceneDataManager.SceneDataStack.SetupElement<T>(force);
            }
        }

        public static List<T> GetAllSceneData<T>() where T : SceneData
        {
            var sceneDataList = new List<T>();

            foreach (SceneDataManager sceneDataManager in SceneDataManagerUtility.GetAllSceneDataManager())
            {
                if (!sceneDataManager.Scene.isLoaded)
                {
                    continue;
                }

                var sceneData = (T)sceneDataManager.SceneDataStack.GetElement(typeof(T));

                if (sceneData != null)
                {
                    sceneDataList.Add(sceneData);
                }
            }

            return sceneDataList;
        }

        public static bool HasMultipleSceneData<T>() where T : SceneData
        {
            var count = 0;

            foreach (SceneDataManager sceneDataManager in SceneDataManagerUtility.GetAllSceneDataManager())
            {
                if (sceneDataManager.SceneDataStack.GetElement(typeof(T)) != null)
                {
                    count++;

                    if (count == 2)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
