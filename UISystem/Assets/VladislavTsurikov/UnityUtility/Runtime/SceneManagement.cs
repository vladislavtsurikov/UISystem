using System.Collections.Generic;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VladislavTsurikov.UnityUtility.Runtime
{
    //This class was created because the EditorSceneManager.activeSceneChanged API was not working at the time of development
    public static class SceneManagement
    {
        private static List<Scene> _pastScenes;

#if UNITY_EDITOR
        public delegate void SceneLoadedOrUnloadedDelegate();

        public static SceneLoadedOrUnloadedDelegate SceneLoadedOrUnloaded;
#else
        public static event UnityAction<Scene, LoadSceneMode> SceneLoaded;
        public static event UnityAction<Scene> SceneUnloaded;
#endif

        static SceneManagement()
        {
#if UNITY_EDITOR
            EditorApplication.update -= CheckLoadedAndUnloadedScenes;
            EditorApplication.update += CheckLoadedAndUnloadedScenes;
#else
            SceneManager.sceneLoaded += SceneLoaded;
            SceneManager.sceneUnloaded += SceneUnloaded;
#endif
        }

#if UNITY_EDITOR
        private static void CheckLoadedAndUnloadedScenes()
        {
            if (_pastScenes == null)
            {
                _pastScenes = GetAllScenes();
                return;
            }

            List<Scene> allScenesLoaded = GetAllScenes();

            foreach (Scene scene in allScenesLoaded)
            {
                if (!_pastScenes.Contains(scene))
                {
                    _pastScenes = allScenesLoaded;
                    SceneLoadedOrUnloaded?.Invoke();

                    return;
                }
            }

            foreach (Scene scene in _pastScenes)
            {
                if (!allScenesLoaded.Contains(scene))
                {
                    _pastScenes = allScenesLoaded;
                    SceneLoadedOrUnloaded?.Invoke();
                    return;
                }
            }
        }
#endif

        public static List<Scene> GetAllScenes()
        {
            var scenes = new List<Scene>();

            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                scenes.Add(SceneManager.GetSceneAt(i));
            }

            return scenes;
        }
    }
}
