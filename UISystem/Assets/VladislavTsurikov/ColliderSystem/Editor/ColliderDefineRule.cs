#if UNITY_EDITOR
using VladislavTsurikov.AutoDefines.Editor;

namespace VladislavTsurikov.ColliderSystem.Editor
{
    public sealed class ColliderDefineRule : StaticDefineRule
    {
        protected override string GetDefineToApplySymbol() => "COLLIDER";
    }
}
#endif
