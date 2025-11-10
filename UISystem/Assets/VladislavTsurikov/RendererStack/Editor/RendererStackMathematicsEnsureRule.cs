#if UNITY_EDITOR
using VladislavTsurikov.AutoDefines.Editor;

namespace VladislavTsurikov.RendererStack.Editor
{
    public sealed class RendererStackMathematicsEnsureDefineRule : PackageDefineRule
    {
        protected override string GetDefineToApplySymbol() { return "RENDERER_STACK_MATHEMATICS"; }
        protected override string GetPackageId() { return "com.unity.mathematics"; }
        protected override bool ShouldAutoInstall() { return true; }
    }
}
#endif
