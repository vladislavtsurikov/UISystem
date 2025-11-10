using UnityEngine.SceneManagement;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;

namespace VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility
{
    public static class SceneObjectsBounds
    {
        public static void ChangeSceneObjectsBounds(Sector sector, bool markSceneDirty = true)
        {
            if (sector == null)
            {
                return;
            }

            sector.ChangeObjectBoundsNodeSize(markSceneDirty);
        }

        public static AABB GetSceneObjectsBounds(Scene scene, bool forceSetupRendererSceneData = false) =>
            GetSceneObjectBounds(SceneDataManagerFinder.Find(scene), forceSetupRendererSceneData);

        public static AABB GetSceneObjectsBounds(Sector sector, bool forceSetupRendererSceneData = false) =>
            GetSceneObjectBounds(sector.SceneDataManager, forceSetupRendererSceneData);

        private static AABB GetSceneObjectBounds(SceneDataManager sceneDataManager,
            bool forceSetupRendererSceneData = false)
        {
            var currentAABB = new AABB();

            if (sceneDataManager == null || !sceneDataManager.IsSetup)
            {
                return currentAABB;
            }

            SceneDataStack sceneDataStack = sceneDataManager.SceneDataStack;

            foreach (SceneData sceneData in sceneDataStack.ElementList)
            {
                if (sceneData is RendererSceneData rendererSceneData)
                {
                    if (forceSetupRendererSceneData)
                    {
                        sceneData.Setup(true);
                    }

                    if (!currentAABB.IsValid)
                    {
                        currentAABB = rendererSceneData.GetAABB();
                        continue;
                    }

                    currentAABB.Encapsulate(rendererSceneData.GetAABB());
                }
            }

            return currentAABB;
        }
    }
}
