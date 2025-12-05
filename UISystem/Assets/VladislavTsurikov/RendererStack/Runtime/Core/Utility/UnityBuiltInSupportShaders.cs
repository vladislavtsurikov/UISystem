using System.Collections.Generic;

namespace VladislavTsurikov.RendererStack.Runtime.Core.Utility
{
    public static class UnityBuiltInSupportShaders
    {
        public static readonly string ShaderGPUIStandard = "RendererStack/Standard";
        public static readonly string ShaderUnityStandard = "Standard";

        public static readonly List<string> StandardUnityShaders = new() { ShaderUnityStandard };

        public static readonly List<string> StandardUnityShadersGPUI = new() { ShaderGPUIStandard };
    }
}
