using Cysharp.Threading.Tasks;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;

namespace VladislavTsurikov.SceneManagerTool.Runtime.Utility
{
    internal static class RuntimeUtility
    {
        internal static void Start()
        {
            Load().Forget();

            static async UniTask Load()
            {
                foreach (SceneCollection sceneCollection in SceneManagerData.Instance.Profile.BuildSceneCollectionStack
                             .ActiveBuildSceneCollection.GetStartupSceneCollections())
                {
                    await sceneCollection.Load();
                }
            }
        }
    }
}
