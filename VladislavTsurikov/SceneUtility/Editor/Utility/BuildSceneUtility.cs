using System;
using System.IO;
using UnityEngine.SceneManagement;

namespace VladislavTsurikov.SceneUtility.Editor.Utility
{
    public static class BuildSceneUtility
    {
        public static bool IsSceneInBuildSettings(string sceneName)
        {
            var sceneCount = SceneManager.sceneCountInBuildSettings;

            for (var i = 0; i < sceneCount; i++)
            {
                var path = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
                var name = Path.GetFileNameWithoutExtension(path);

                if (string.Equals(name, sceneName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
