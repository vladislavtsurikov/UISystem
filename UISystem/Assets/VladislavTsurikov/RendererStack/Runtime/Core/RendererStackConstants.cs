using VladislavTsurikov.Core.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.Core
{
    public static class RendererStackConstants
    {
        private const string RendererStack = "RendererStack";
        private const string ShadersName = "Shaders";
        public static readonly float Version = 1.05f;

        public static readonly string PathToResourcesRendererStack =
            CommonPath.CombinePath(CommonPath.PathToResources, RendererStack);

        public static readonly string PathToShaders = CommonPath.CombinePath(PathToResourcesRendererStack, ShadersName);

        #region Shaders

        public static readonly string ShaderUnityInternalError = "Hidden/InternalErrorShader";

        #endregion Shaders
    }
}
