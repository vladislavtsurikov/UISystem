using System.Collections.Generic;
using VladislavTsurikov.SceneManagerTool.Runtime.BuildSceneCollectionSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;

namespace VladislavTsurikov.SceneManagerTool.Runtime.Utility
{
    public static class SceneCollectionUtility
    {
        public static SceneCollection GetSceneCollection(int id)
        {
            foreach (BuildSceneCollection buildSceneCollection in
                     SceneManagerData.Instance.Profile.BuildSceneCollectionStack.ElementList)
            foreach (SceneCollection sceneCollection in buildSceneCollection.GetAllSceneCollections())
            {
                if (sceneCollection.ID == id)
                {
                    return sceneCollection;
                }
            }

            return null;
        }

        public static List<SceneCollection> GetAllCurrentSceneCollection()
        {
            var sceneCollections = new List<SceneCollection>();
            foreach (BuildSceneCollection buildSceneCollection in
                     SceneManagerData.Instance.Profile.BuildSceneCollectionStack.ElementList)
            foreach (SceneCollection sceneCollection in buildSceneCollection.GetAllSceneCollections())
            {
                sceneCollections.Add(sceneCollection);
            }

            return sceneCollections;
        }
    }
}
