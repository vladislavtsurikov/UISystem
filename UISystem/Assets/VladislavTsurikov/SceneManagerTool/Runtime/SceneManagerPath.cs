using VladislavTsurikov.Core.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime
{
    public static class SceneManagerPath
    {
        private const string SceneManager = "SceneManager";

        public static readonly string PathToResourcesSceneManager =
            CommonPath.CombinePath(CommonPath.PathToResources, SceneManager);
    }
}
