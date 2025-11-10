#if UNITY_EDITOR
using VladislavTsurikov.AutoDefines.Editor;

namespace VladislavTsurikov.ZenjectUtility.Editor
{
    public sealed class ZenjectUtilityRule : TypeDefineRule
    {
        protected override string GetDefineToApplySymbol() => "ZENJECT_UTILITY";
        public override string GetTypeFullName() => "Zenject.DiContainer";
    }
}
#endif

