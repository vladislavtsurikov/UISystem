#if UNITY_EDITOR
using VladislavTsurikov.AutoDefines.Editor;

namespace VladislavTsurikov.RendererStack.Editor
{
    public sealed class RendererStackDefineRule : StaticDefineRule
    {
        protected override string GetDefineToApplySymbol() => "RENDERER_STACK";
    }
}
#endif
