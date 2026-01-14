#if UNITY_EDITOR
using VladislavTsurikov.AutoDefines.Editor;

namespace VladislavTsurikov.DOTweenUtility.Editor
{
    public sealed class DOTweenUtilityRule : TypeDefineRule
    {
        protected override string GetDefineToApplySymbol() => "DOTWEEN_UTILITY";
        public override string GetTypeFullName() => "DG.Tweening.DOTween";
    }
}
#endif
