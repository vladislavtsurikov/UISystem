using VladislavTsurikov.ScriptableObjectUtility.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.Core.Preferences
{
    [LocationAsset("RendererStack/RendererStackSettings")]
    public class RendererStackSettings : SerializedScriptableObjectSingleton<RendererStackSettings>
    {
        public float Version;

        public bool AutoShaderConversion = true;
        public bool ShowRenderModelData;
        public bool RenderSceneCameraInPlayMode;
        public bool RenderDirectToCamera;

        public bool ShowColliders = true;
        public bool RenderImposter = true;
        public bool ForceUpdateRendererData = true;
    }
}
