using Cysharp.Threading.Tasks;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneTypeSystem;

namespace VladislavTsurikov.SceneManagerTool.Runtime
{
    public static class SceneManagerAPI
    {
        public static void UnloadSceneCollection(SceneCollection sceneCollection) =>
            sceneCollection.Unload(null).Forget();

        public static void LoadSceneCollection(SceneCollection sceneCollection) => sceneCollection.Load().Forget();

        public static void LoadSceneType(SceneType sceneType) => sceneType.LoadInternal(true).Forget();

        public static void UnloadSceneComponent(SceneType sceneType) => sceneType.UnloadInternal(null, true).Forget();
    }
}
