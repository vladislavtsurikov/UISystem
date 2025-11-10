#if UNITY_EDITOR
using VladislavTsurikov.AutoDefines.Editor;

namespace VladislavTsurikov.RendererStack.Editor
{
    public sealed class RendererStackBurstDefineRule : PackageDefineRule
    {
        protected override string GetDefineToApplySymbol() { return "RENDERER_STACK_BURST"; }
        protected override string GetPackageId() { return "com.unity.burst"; }
        protected override bool ShouldAutoInstall() { return false; }
    }
}
#endif
