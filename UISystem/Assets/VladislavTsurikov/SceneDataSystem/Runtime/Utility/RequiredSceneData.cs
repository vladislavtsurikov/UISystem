using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.SceneDataSystem.Runtime.Utility
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class RequiredSceneData
    {
        private static readonly List<Type> _requiredTypeList = new();

        static RequiredSceneData()
        {
            IEnumerable<Type> types = AllTypesDerivedFrom<SceneData>.Types
                .Where(t => t.IsDefined(typeof(RequiredSceneDataAttribute), false));

            foreach (Type type in types)
            {
                _requiredTypeList.Add(type);
            }
        }

        public static void AddRequiredType(Type type)
        {
            if (!_requiredTypeList.Contains(type))
            {
                _requiredTypeList.Add(type);
            }

            CreateAllRequiredTypes();
        }

        public static void CreateAllRequiredTypes()
        {
            foreach (SceneDataManager sceneDataManager in SceneDataManagerUtility.GetAllSceneDataManager())
            {
                if (!sceneDataManager.IsSetup)
                {
                    sceneDataManager.Setup();
                }

                foreach (Type type in _requiredTypeList)
                {
                    sceneDataManager.SceneDataStack.CreateIfMissingType(type);
                }
            }
        }

        public static void Create<T>() where T : SceneData
        {
            foreach (SceneDataManager sceneDataManager in SceneDataManagerUtility.GetAllSceneDataManager())
            {
                if (!sceneDataManager.IsSetup)
                {
                    sceneDataManager.Setup();
                }

                sceneDataManager.SceneDataStack.CreateIfMissingType(typeof(T));
            }
        }
    }
}
