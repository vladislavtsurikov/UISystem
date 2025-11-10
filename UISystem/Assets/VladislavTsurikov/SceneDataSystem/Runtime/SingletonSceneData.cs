using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;

namespace VladislavTsurikov.SceneDataSystem.Runtime
{
    public class SingletonSceneData<T> : SceneData where T : SceneData
    {
        private static bool _setup;
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (Get() != null)
                {
                    return _instance;
                }

                if (SceneDataStackUtility.HasMultipleSceneData<T>())
                {
                    Debug.LogError("There are several types of " + typeof(T) +
                                   " at the level, but there should be only one");
                    return null;
                }

                RequiredSceneData.Create<T>();
                return Get();
            }
        }

        private static T Get()
        {
            if (!_setup)
            {
                Setup();
            }

            if (!_setup || _instance == null || _instance.SceneDataManager == null ||
                !_instance.SceneDataManager.Scene.isLoaded)
            {
                _setup = true;
                List<T> sceneDatas = SceneDataStackUtility.GetAllSceneData<T>();

                if (sceneDatas.Count > 1)
                {
                    Debug.LogError("There are several types of " + typeof(T) +
                                   " at the level, but there should be only one");
                    _instance = null;
                    return null;
                }

                if (sceneDatas.Count == 1)
                {
                    _instance = sceneDatas[0];
                    return _instance;
                }

                _instance = null;
                return _instance;
            }

            return _instance;
        }

        private static void Setup()
        {
            OnSetupEvent -= CheckValid;
            OnSetupEvent += CheckValid;
        }

        private static void CheckValid() => _setup = false;
    }
}
